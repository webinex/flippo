import axios, { AxiosInstance } from 'axios';

export interface FlippoStoreArgs {
  file: File;
  reference?: string;
  replace?: boolean;
}

export interface FlippoOptions {
  axios?: AxiosInstance;
}

const DEFAULT_OPTIONS: FlippoOptions = {
  axios: axios.create({
    baseURL: '/api/flippo',
  }),
};

class FlippoHttp {
  private _axios: AxiosInstance;

  constructor(axios: AxiosInstance) {
    if (axios == null) throw new Error('`axios` might not be null');

    this._axios = axios;
  }

  public store = async (args: FlippoStoreArgs): Promise<string> => {
    if (args == null) throw new Error('`args` might not be null');
    if (args.file == null) throw new Error('`args.file` might not be null');

    const { file, reference, replace } = args;

    const form = new FormData();
    form.append('file', file);

    const params = [];
    if (reference) {
      params.push(`reference=${encodeURIComponent(reference)}`);
    }
    if (replace != null) {
      params.push(`replace=${encodeURIComponent(replace)}`);
    }

    const uri = params.length > 0 ? `?${params.join('&')}` : '';

    const response = await this._axios.post<string>(uri, form);
    return response.data;
  };

  public fetch = async (reference: string): Promise<Blob> => {
    if (!reference) throw new Error('`reference` might not be null');

    const response = await this._axios({
      url: reference,
      method: 'GET',
      responseType: 'blob',
    });

    const data: Blob = response.data;
    return data;
  };

  public delete = async (reference: string): Promise<void> => {
    if (!reference) throw new Error('`reference` might not be null');

    await this._axios.delete(reference);
  };
}

export class Flippo {
  private _http: FlippoHttp;

  constructor(options: FlippoOptions = DEFAULT_OPTIONS) {
    options = { ...DEFAULT_OPTIONS, ...options };

    this._http = new FlippoHttp(options.axios);
  }

  public fetch = async (reference: string): Promise<Blob> => {
    if (reference == null) throw new Error('`reference` might not be null');

    return await this._http.fetch(reference);
  };

  public store = async (
    fileOrArgs: File | FlippoStoreArgs
  ): Promise<string> => {
    if (fileOrArgs == null) throw new Error('`fileOrArgs` might not be null');

    const args = this.storeArgs(fileOrArgs);
    return await this._http.store(args);
  };

  private storeArgs = (fileOrArgs: File | FlippoStoreArgs): FlippoStoreArgs => {
    if (fileOrArgs instanceof File) {
      return { file: fileOrArgs };
    }

    if (fileOrArgs.file == null)
      throw new Error('`fileOrArgs.file` might not be null');
    return fileOrArgs;
  };

  public delete = async (reference: string): Promise<void> => {
    if (!reference) throw new Error('`reference` might not be null');

    return await this._http.delete(reference);
  };
}
