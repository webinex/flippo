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

export class Flippo {
  private _options: FlippoOptions;
  private _axios: AxiosInstance;

  constructor(options: FlippoOptions = DEFAULT_OPTIONS) {
    this._options = { ...DEFAULT_OPTIONS, ...options };
    this._axios = this._options.axios!;
  }

  public fetch = async (
    reference: string,
    cacheControlMaxAge?: number,
    etag?: boolean
  ): Promise<Blob> => {
    if (!reference) {
      throw new Error('`reference` might not be null');
    }

    const searchParams = new URLSearchParams();

    if (cacheControlMaxAge !== undefined) {
      searchParams.append('cacheControlMaxAge', cacheControlMaxAge.toString());
    }

    if (etag !== undefined) {
      searchParams.append('etag', etag.toString());
    }

    const response = await this._axios({
      url: reference + '?' + searchParams.toString(),
      method: 'GET',
      responseType: 'blob',
    });

    const data: Blob = response.data;
    return data;
  };

  public getSasToken = async (reference: string): Promise<string> => {
    if (!reference) throw new Error('`reference` might not be null');

    const { data } = await this._axios.get(`${reference}/sas-token`);
    return data;
  };

  public getSasUrl = async (reference: string): Promise<string> => {
    const token = await this.getSasToken(reference);

    return (
      this._options.axios!.defaults.baseURL! +
      `/${reference}/open?token=${encodeURIComponent(token)}`
    );
  };

  public store = async (
    fileOrArgs: File | FlippoStoreArgs
  ): Promise<string> => {
    const { file, reference, replace } = !(fileOrArgs instanceof File)
      ? fileOrArgs
      : { file: fileOrArgs, replace: false, reference: undefined };

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

  public delete = async (reference: string): Promise<void> => {
    if (!reference) throw new Error('`reference` might not be null');

    await this._axios.delete(reference);
  };
}
