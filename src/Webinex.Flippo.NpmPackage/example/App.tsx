import * as React from 'react';
import './App.css';
import { FileControl } from './FileControl';
import { FileList } from './FileList';
import { useFileInfoArray } from './useFileInfoArray';

export function App() {
  const [files] = useFileInfoArray({ preload: true });

  return (
    <div className="main">
      <div className="content">
        <div className="title">@webinex/flippo</div>

        <div className="card">
          <div className="form-group">
            <div className="form-label">Select file to upload</div>
            <div className="form-control">
              <FileControl />
            </div>
          </div>
        </div>
        {files.length > 0 && (
          <div className='card'>
            <FileList items={files} />
          </div>
        )}
      </div>
    </div>
  );
}
