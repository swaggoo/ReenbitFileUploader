import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavComponent } from './nav/nav.component';
import { NgxSpinnerModule } from 'ngx-spinner';



@NgModule({
  declarations: [
    NavComponent,
  ],
  imports: [
    CommonModule,
    NgxSpinnerModule
  ],
  exports: [
    NavComponent,
    NgxSpinnerModule
  ]
})
export class CoreModule { }
