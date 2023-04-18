import { NgModule } from "@angular/core";
import { ModalComponent } from 'src/app/views/pages/ui-components/modal/modal.component';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import { EmojiModule } from '@ctrl/ngx-emoji-mart/ngx-emoji';

@NgModule({
  declarations:[
  ],
  imports:[
    PickerModule,
    EmojiModule
  ],
  exports: [
    PickerModule,
    EmojiModule
  ]
})
export class SharedModule{}
