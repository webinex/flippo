import * as React from 'react';
import { useCallback } from 'react';
import { useAppFlippo } from './useAppFlippo';

export interface FileInfo {
  name: string;
  reference: string;
}

interface Props {
  items: FileInfo[];
}

function useOpen() {
  const flippo = useAppFlippo();

  return useCallback(
    async (reference: string) => {
      const url = await flippo.getSasUrl(reference);
      const newWindow = window.open(url, '_blank');

      setTimeout(() => {
        newWindow!.document.title = 'Flippo / Document';
      }, 10);
    },
    [flippo]
  );
}

export function FileList(props: Props) {
  const { items } = props;
  const open = useOpen();

  if (items.length === 0) {
    return null;
  }

  return (
    <div className="file-list">
      {items.map(x => (
        <div className="file-list-item" key={x.reference}>
          <div>
            <a href="javascript:void(0)" onClick={() => open(x.reference)}>
              {x.name}
            </a>
          </div>
          <div className="file-list-item-ref">{x.reference}</div>
        </div>
      ))}
    </div>
  );
}
