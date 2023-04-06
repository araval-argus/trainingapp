import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from 'src/app/core/service/account-service';

@Component({
  selector: 'app-search-dropdown',
  templateUrl: './search-dropdown.component.html',
  styleUrls: ['./search-dropdown.component.scss']
})
export class SearchDropdownComponent implements OnInit {

  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  @Input() items: any[];
  @Output() selectedItem = new EventEmitter<any>();

  selectItem(item: any) {
    this.selectedItem.emit(item);
  }

  getImage(imagePath: string) {
    if (imagePath == null) {
      return "https://via.placeholder.com/37x37";
    }
    return this.accountService.fetchImage(imagePath);
  }
}
