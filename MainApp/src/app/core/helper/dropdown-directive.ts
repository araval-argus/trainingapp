import { Directive, EventEmitter, HostListener, Output } from "@angular/core";


@Directive({
  selector: '[appDropdown]'
})
export class DropdownDirective{

  @Output() hideDropdown = new EventEmitter();

  @HostListener('document:click',['$event']) closeDropdown(event: MouseEvent){
    const appDropdown = document.querySelector('.appDropdown');
    const searchInput = document.querySelector('#searchForm');

    // if clicked on the elements except input field or dropdown
    if(event.target !== searchInput && event.target !== appDropdown){
      this.hideDropdown.emit();
    }
  }
}
