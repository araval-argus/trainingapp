import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { PerfectScrollbarDirective } from 'ngx-perfect-scrollbar';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ChatModel } from 'src/app/core/models/chat-model';
import { loadChatModel } from 'src/app/core/models/loadingChat-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { Profile } from 'src/app/core/models/profile-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, AfterViewInit, OnDestroy, AfterViewChecked {

  loggedInUser: LoggedInUser
  imageURL: string;
  profiles: Profile[]
  defaultNavActiveId = 1;
  private searchSubscription?: Subscription;
  private reloadNewInbox?: Subscription;
  searchQuery: string;
  removeResult: boolean = false;
  hideRightBox: boolean = true;
  selectedUser: Profile = {
    firstName: '',
    lastName: '',
    userName: '',
  }
  chat: ChatModel;
  chatsToLoad = new Array<loadChatModel>();
  selectedUserImagePath: string = null;
  @ViewChild('chatForm', { static: false }) chatContent: ElementRef;

  // To convert user input to the observable
  private readonly searchProfile = new Subject<string | undefined>();
  private readonly reloadInbox = new Subject<Event>();


  constructor(private authService: AuthService, private accountService: AccountService, private chatService: ChatService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.imageURL = localStorage.getItem('imagePath');
    if (this.imageURL == null) {
      this.imageURL = "https://via.placeholder.com/37x37";
    }
    this.searchSubscription = this.searchProfile.pipe(
      debounceTime(1000),//operator waits for 1000 milliseconds
      distinctUntilChanged(),//search query is different from the previous search
      switchMap((searchQuery) => this.accountService.searchProfiles(searchQuery)),//operator will cancel any ongoing search request if a new search query is emitted, and only the latest search request will be sent to the server.
    ).subscribe(data => {
      this.profiles = data;
    });


    // This will update User every time new user selected
    this.reloadNewInbox = this.reloadInbox.subscribe((event: any) => {
      this.selectedUser.firstName = event.firstName;
      this.selectedUser.lastName = event.lastName;
      this.selectedUser.userName = event.userName;
      this.selectedUser.imagePath = event.imagePath;
      if (event.imagePath != null) {
        this.selectedUserImagePath = this.accountService.fetchImage(this.selectedUser.imagePath);
      } else {
        this.selectedUserImagePath = "https://via.placeholder.com/43x43";
      }
    })
  }

  @ViewChild('scrollMe') scrollMe: ElementRef
  ngAfterViewChecked(): void {
    try {
      this.scrollMe.nativeElement.scrollTop =
        this.scrollMe.nativeElement.scrollHeight;
    } catch (err) { }
  }


  ngAfterViewInit(): void {
    // Show chat-content when clicking on chat-item for tablet and mobile devices
    document.querySelectorAll('.chat-list .chat-item').forEach(item => {
      item.addEventListener('click', event => {
        document.querySelector('.chat-content').classList.toggle('show');
      })
    });

  }


  ngOnDestroy(): void {
    this.searchSubscription.unsubscribe();
    this.reloadNewInbox.unsubscribe();
  }

  // back to chat-list for tablet and mobile devices
  backToChatList() {
    document.querySelector('.chat-content').classList.toggle('show');
  }

  save() {
    console.log('passs');
  }

  fetchProfile(event: Event) {
    this.searchQuery = (event.target as HTMLInputElement).value;
    this.removeResult = false;
    if (this.searchQuery.length >= 3) {
      this.searchProfile.next(this.searchQuery?.trim());
    } else {
      this.profiles = [];
    }
  }

  loadInbox(event) {
    this.removeResult = true;
    this.hideRightBox = false;
    this.reloadInbox.next(event);
    this.reloadChat();
  }

  // Purpose of not to set sender here is security, intruder can change the sender username in request
  sendChat() {
    const chat = this.chatContent.nativeElement.value;
    this.chatContent.nativeElement.value = "";
    this.chat = {
      to: this.selectedUser.userName,
      content: chat,
    }
    this.chatService.addChat(this.chat).subscribe(data => {
      this.reloadChat();
    })
  }


  //Load new Chat
  reloadChat() {
    this.chatsToLoad = [];
    this.chatService.getChat(this.selectedUser.userName).subscribe((data: any) => {
      this.loadChat(data);
    })
  }

  // This Method will convert CHATDTO to chat model to render on screen.
  loadChat(data: any) {
    var sent = data.chats[0];
    var recieved = data.chats[1];
    if (sent.chatList != null) {
      sent.chatList.forEach(element => {
        this.chatsToLoad.push({
          sent: true,
          content: element.content,
          sentAt: new Date(element.sentAt)
        })
      });
    }
    if (recieved.chatList != null) {
      recieved.chatList.forEach(element => {
        this.chatsToLoad.push({
          sent: false,
          content: element.content,
          sentAt: new Date(element.sentAt)
        })
      });
    }
    this.chatsToLoad.sort((a, b) => a.sentAt.getTime() - b.sentAt.getTime());
  }
}
