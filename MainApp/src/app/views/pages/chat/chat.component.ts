import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { PerfectScrollbarDirective } from 'ngx-perfect-scrollbar';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, switchMap } from 'rxjs/operators';
import { ChatModel } from 'src/app/core/models/chat-model';
import { loadChatModel } from 'src/app/core/models/loadingChat-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { Profile } from 'src/app/core/models/profile-model';
import { recentChat } from 'src/app/core/models/recentChat-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { HubService } from 'src/app/core/service/hub-service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, AfterViewInit, OnDestroy, AfterViewChecked {

  loggedInUser: LoggedInUser

  removeResult: boolean = false;
  hideRightBox: boolean = true;

  imageURL: string;
  searchQuery: string;

  profiles: Profile[]
  recentChatProfile: recentChat[] = [];
  defaultNavActiveId = 1;
  //To store message to be replied

  chat: ChatModel;

  //Manages subscription to be unsubscribe on destroy
  private reloadNewInbox?: Subscription;



  @ViewChild('chatForm', { static: false }) chatContent: ElementRef;
  @ViewChild('searchBar', { static: false }) searchBar: ElementRef;

  // To convert user input to the observable
  private readonly searchProfile = new Subject<string | undefined>();

  //To manage selected User
  selectedUser: any = null;
  removeRightBox: boolean = false;

  constructor(private authService: AuthService, private accountService: AccountService, private chatService: ChatService) {
  }

  ngOnInit(): void {
    // To load loggedIn User's data
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    //When logged In Fetch the image that has been saved while logging in
    this.imageURL = localStorage.getItem('imagePath');
    if (this.imageURL == null) {
      //If image is not saved than replace it with thumbnail
      this.imageURL = "https://via.placeholder.com/37x37";
    }

    //We have used user Defined Subject to adding Pipe and manipulating query
    this.searchProfile.pipe(
      debounceTime(1000),//operator waits for 1000 milliseconds
      switchMap((searchQuery) => this.accountService.searchProfiles(searchQuery)),//operator will cancel any ongoing search request if a new search query is emitted, and only the latest search request will be sent to the server.
    ).subscribe(data => {
      this.profiles = data;
    });


    // This will update selected User every time new user selected
    // Since search Result and side bar (Recent Chat) is in other component we have used subscription

    this.reloadNewInbox = this.chatService.reloadInbox.subscribe((data) => {
      //Hide search result on selecting one user
      this.profiles = [];
    })
  }

  //To navigate to bottom of the list
  ngAfterViewChecked(): void {

  }

  ngAfterViewInit(): void {

    // Show chat-content when clicking on chat-item for tablet and mobile devices
    document.querySelectorAll('.chat-list .chat-item').forEach(item => {
      item.addEventListener('click', () => {
        document.querySelector('.chat-content').classList.toggle('show');
      })
    });
  }


  ngOnDestroy(): void {
    this.reloadNewInbox.unsubscribe();
  }

  // back to chat-list for tablet and mobile devices
  backToChatList() {
    document.querySelector('.chat-content').classList.toggle('show');
  }

  save() {
    console.log('passs');
  }

  //This will search the user based on search string
  fetchProfile(event: Event) {
    this.searchQuery = (event.target as HTMLInputElement).value;
    this.removeResult = false;
    if (this.searchQuery.length >= 3) {
      this.searchProfile.next(this.searchQuery?.trim());
    } else {
      this.profiles = [];
    }
  }

  //This will load inbox based on selected user from search Result
  loadInbox(event) {
    this.chatService.reloadInbox.next(event);
    (this.searchBar.nativeElement as HTMLInputElement).value = "";
  }
}