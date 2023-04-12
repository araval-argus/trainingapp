import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-file-modal',
  templateUrl: './file-modal.component.html',
  styleUrls: ['./file-modal.component.scss']
})
export class FileModalComponent implements OnInit {

  url;
  unsafeUrl;
  format;
  file: File;
  @ViewChild('basicModal', { static: false }) content: TemplateRef<any>;
  constructor(private modalService: NgbModal, private chatService: ChatService, private domSanitizer: DomSanitizer) { }

  ngOnInit(): void {
    this.chatService.displayModal.subscribe(data => {
      this.file = data;
      this.convertFile();
      this.displayModal();
    })
  }

  convertFile() {
    var reader = new FileReader();
    reader.readAsDataURL(this.file);
    if (this.file.type.indexOf('image') > -1) {
      this.format = 'image';
    } else if (this.file.type.indexOf('video') > -1) {
      this.format = 'video';
    } else if (this.file.type.indexOf('audio') > -1) {
      this.format = 'audio';
    }
    reader.onload = (event) => {
      this.unsafeUrl = (<FileReader>event.target).result;
      this.url = this.domSanitizer.bypassSecurityTrustResourceUrl(this.unsafeUrl);
    }
  }

  displayModal() {
    this.modalService.open(this.content, {}).result.then(() => {
    }).catch(() => {
      console.log("Closed");
    });
  }

  cancel() {
  }

  send() {
    this.chatService.sendFileSub.next();
  }

}
