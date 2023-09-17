import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { FileAccessUrl } from '../core/models/fileAcessUrl';


@Injectable({
  providedIn: 'root'
})
export class FilesService {
  baseUrl = environment.apiUrl

  constructor(private http: HttpClient) { }

  uploadFile(values: any) {
    return this.http.post<FileAccessUrl>(this.baseUrl + 'files', values);
  }
}
