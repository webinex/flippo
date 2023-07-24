import * as React from 'react';
import { useCallback, useRef, useState } from 'react';
import { FileList } from './FileList';
import { useAppFlippo } from './useAppFlippo';
import { useFileInfoArray } from './useFileInfoArray';

function useControl() {
  const flippo = useAppFlippo();
  const ref = useRef<HTMLInputElement | null>(null!);
  const [uploading, setUploading] = useState<File[]>([]);
  const [files, setFiles] = useFileInfoArray({ preload: false })
  const open = useCallback(() => ref.current?.click(), []);

  const onChange = useCallback(
    async (e: React.ChangeEvent<HTMLInputElement>) => {
      const files = Array.from(e.target.files!);
      setUploading(files);

      files.forEach(file =>
        flippo
          .store(file)
          .then(reference =>
            setFiles(files => [...files, { name: file.name, reference }])
          )
          .then(() => setUploading(values => values.filter(x => x !== file)))
      );
    },
    [flippo]
  );

  return { ref, onChange, open, uploading, files };
}

function FileUploadingList(props: { items: File[] }) {
  const { items } = props;

  if (items.length === 0) {
    return null;
  }

  return (
    <div className="file-uploading-list">
      {items.map((x, index) => (
        <div className="file-uploading-list-item" key={index}>
          {x.name}........
        </div>
      ))}
    </div>
  );
}


export function FileControl() {
  const { ref, uploading, files, onChange, open } = useControl();

  return (
    <div className="file-control">
      <div className="file-input" onClick={open}>
        <input type="file" multiple ref={ref} value="" onChange={onChange} />
      </div>

      <FileUploadingList items={uploading} />
      <FileList items={files} />
    </div>
  );
}
