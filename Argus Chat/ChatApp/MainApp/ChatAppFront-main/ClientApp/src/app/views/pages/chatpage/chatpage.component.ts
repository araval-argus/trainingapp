import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatComponent } from '../../layout/chat/chat.component';
import { SidebarComponent } from '../../layout/sidebar/sidebar.component';
import { ChatService } from '../../../services/chat/chat.service';
import { AuthService } from '../../../services/auth/auth.service';
import { Subscription } from 'rxjs';
import { ViewProfile } from 'src/app/models/view-profile.model';

@Component({
  selector: 'app-chatpage',
  standalone: true,
  imports: [CommonModule,ChatComponent,SidebarComponent],
  templateUrl: './chatpage.component.html',
  styleUrls: ['./chatpage.component.css']
})
export class ChatpageComponent {
  conversation :boolean = false;
  Messages: string[] = [];
  conversations = [
  ];
  currentUserId :any ;
  currentUserName : any;
  otheruserName : string;
  imageFile: File;
  profile : ViewProfile;
  private subscription: Subscription;

  constructor(private chatService: ChatService, private authService: AuthService) {
    console.log("this is done", this.currentUserName);
    
    this.subscription = this.chatService.Username.subscribe((message) => {
      
      this.conversation = true;
    });
  }

  
  ngOnInit(): void {
    
     this.currentUserName = localStorage.getItem('username');
     console.log(this.currentUserName);
     this.authService.getdetails(this.currentUserName);

    this.chatService.GetUserId(this.currentUserName).subscribe((id)=>{
      this.currentUserId = id;      
   })
   

    this.chatService.Username.subscribe(data => {
      // console.log("insid")
     this.otheruserName = data;

     this.chatService.viewMessages(this.currentUserName,data).subscribe((message)=>{
        this.conversations = message.reverse();
        console.log(this.conversations);
     }, (error) => {
      console.log("ff ",error)
     })
    })
 }

 
  
  handleData(data: any[]) {
       console.log(data);

       this.conversations = [
        {
          content: data[0],
          senderId: this.currentUserId,
          recieverId: data[1],
          dateTime: Date.now(),
          isReply: false,
          isSeen: false,
          replyedToId: 0,
          type: 'Null'
        },
        ...this.conversations
      ];
      
       console.log("ggg ", this.conversations);
  }
  
  ngOnDestroy() {
    // Unsubscribe to avoid memory leaks
    this.subscription.unsubscribe();
  }
}
