import { Component, OnInit } from '@angular/core';
import { FilesService } from './files.service';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.css']
})
export class FilesComponent {
  fileToUpload?: File;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private filesService: FilesService
    ) {}

  fileForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  })

  onSubmit() {
    this.loading = true;
  
    if (this.isFormDataValid()) {
      const formData = this.createFormData();
      this.uploadFormData(formData);
    } else {
      console.log('Form data is incomplete');
    }
  }
  
  isFormDataValid() {
    const emailControl = this.fileForm.get('email');
    return emailControl && emailControl.value && this.fileToUpload;
  }
  
  createFormData() {
    const formData = new FormData();
    const emailControl = this.fileForm.get('email');

    if (emailControl && emailControl.value && this.fileToUpload) {
      formData.append('email', emailControl.value);
      formData.append('file', this.fileToUpload);
    }

    return formData;
  }
  
  uploadFormData(formData: FormData) {
    this.filesService.uploadFile(formData).subscribe({
      next: (token: any) => { 
        this.updateAccessUrlTextarea(token.url);
        this.loading = false;
      },
      error: (error: any) => { 
        console.log(error);
        this.loading = false;
      }
    });
  }
  
  updateAccessUrlTextarea(url: string) {
    const accessUrlTextarea = document.getElementById('accessUrl') as HTMLTextAreaElement;
    if (accessUrlTextarea) {
      accessUrlTextarea.value = url;
    }
  }

  handleFileInput(e: any) {
    this.fileToUpload = e?.target?.files[0];
  }
}
