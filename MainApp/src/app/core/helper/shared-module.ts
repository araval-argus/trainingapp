import { NgModule } from "@angular/core";
import { ModalComponent } from 'src/app/views/pages/ui-components/modal/modal.component';

@NgModule({
  declarations:[
    ModalComponent
  ],
  exports: [
    ModalComponent
  ]
})
export class SharedModule{}
