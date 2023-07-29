import { useEffect, useState } from 'react';
import { FileInfo } from './FileList';

const PREFIX = 'file-';

export function useFileInfoArray(options: { preload: boolean }) {
  const { preload } = options;

  const [files, setFiles] = useState<FileInfo[]>(() => {
    if (!preload) {
      return [];
    }

    return Object.entries(localStorage)
      .filter(([key]) => key.startsWith(PREFIX))
      .map(([key, value]) => ({
        name: value,
        reference: key.substring(PREFIX.length),
      }));
  });

  useEffect(() => {
    files.forEach(({ name, reference }) =>
      localStorage.setItem(`${PREFIX}${reference}`, name)
    );
  }, [files]);

  return [files, setFiles] as const;
}
