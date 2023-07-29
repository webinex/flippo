import * as React from 'react';
import { useCallback } from 'react';
import { useAppFlippo } from './useAppFlippo';
import Clipboard from 'jsx:./clipboard.svg';
import Download from 'jsx:./download.svg';
import { toast } from './toast';

export interface FileInfo {
  name: string;
  reference: string;
}

interface Props {
  items: FileInfo[];
}

function useOpenSas() {
  const flippo = useAppFlippo();

  return useCallback(
    async (reference: string) => {
      const token = await flippo.getSasToken(reference);
      const now = new Date();
      now.setSeconds(now.getSeconds() + 60);

      document.cookie = `__FLIPPO_Token=${token}; expires=${now.toUTCString()}; path=/api/flippo/${reference}/open`;
      const newWindow = window.open(
        `/api/flippo/${reference}/open?etag=true&cacheControlMaxAge=60`,
        '_blank'
      );

      setTimeout(() => {
        newWindow!.document.title = 'Flippo / Document';
      }, 10);
    },
    [flippo]
  );
}

function useDownload() {
  const flippo = useAppFlippo();

  return useCallback(
    async (reference: string) => {
      toast('≥: Downloading...');
      const blob = await flippo.fetch(reference, 120, true);
      toast('≥: Downloaded!');

      const url = URL.createObjectURL(blob);
      window.open(url, '_blank');

      setTimeout(() => {
        URL.revokeObjectURL(url);
      }, 100);
    },
    [flippo]
  );
}

function useCopyURL() {
  const flippo = useAppFlippo();

  return useCallback(
    async (reference: string) => {
      const token = await flippo.getSasToken(reference);
      const now = new Date();
      now.setSeconds(now.getSeconds() + 60);

      document.cookie = `__FLIPPO_Token=${token}; expires=${now.toUTCString()}; path=/api/flippo/${reference}/open`;
      navigator.clipboard.writeText(
        `${window.location.origin}/api/flippo/${reference}/open?etag=true&cacheControlMaxAge=60`
      );
      toast('≥: Copied to clipboard!');
    },
    [flippo]
  );
}

export function FileList(props: Props) {
  const { items } = props;
  const openSas = useOpenSas();
  const copyURL = useCopyURL();
  const download = useDownload();

  if (items.length === 0) {
    return null;
  }

  return (
    <div className="file-list">
      {items.map(x => (
        <div className="file-list-item" key={x.reference}>
          <div className="file-list-item-head">
            <button
              title="Open SAS (Shared access signature)"
              className="btn-link"
              onClick={() => openSas(x.reference)}
            >
              {x.name}
            </button>
            <span>
              <button
                title="Download as Blob"
                onClick={() => download(x.reference)}
                className="btn-icon"
              >
                <Download />
              </button>
              <button
                title="Copy to clipboard"
                onClick={() => copyURL(x.reference)}
                className="btn-icon"
              >
                <Clipboard />
              </button>
            </span>
          </div>
          <div className="file-list-item-ref">{x.reference}</div>
        </div>
      ))}
    </div>
  );
}
