import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { Profile } from 'src/app/core/models/profile-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, AfterViewInit, OnDestroy {

  loggedInUser: LoggedInUser
  imageURL: string;
  profiles: Profile[]
  defaultNavActiveId = 1;
  private searchSubscription?: Subscription;
  searchQuery: string;
  removeResult: boolean = false;

  // To convert user input to the observable
  private readonly searchProfile = new Subject<string | undefined>();

  constructor(private authService: AuthService, private accountService: AccountService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.imageURL = localStorage.getItem('imagePath');
    this.searchSubscription = this.searchProfile.pipe(
      debounceTime(1000),//operator waits for 1000 milliseconds
      distinctUntilChanged(),//search query is different from the previous search
      switchMap((searchQuery) => this.accountService.searchProfiles(searchQuery)),//operator will cancel any ongoing search request if a new search query is emitted, and only the latest search request will be sent to the server.
    ).subscribe(data => {
      this.profiles = data;
    });
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


  selectItem(event) {
    console.log(event);
    this.removeResult = true;
  }
}
