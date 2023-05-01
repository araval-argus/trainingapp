import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { HubService } from 'src/app/core/service/hub-service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-status',
  templateUrl: './status.component.html',
  styleUrls: ['./status.component.scss']
})
export class StatusComponent implements OnInit {


  status = [
    { id: 1, status: 'Available', class: 'mdi-bell-ring active' },
    { id: 2, status: 'Busy', class: 'mdi-minus-circle hold' },
    { id: 3, status: 'Do Not Disturb', class: 'mdi-bell-cancel hold' },
    { id: 4, status: 'Be Right Back', class: 'mdi-clock away' },
    { id: 5, status: 'Appear Away', class: 'mdi-shoe-sneaker away' },
    { id: 6, status: 'Appear Offline', class: 'mdi-location-exit' },
  ];

  selectedStatus = 1;
  constructor(private accountService: AccountService, private hubService: HubService, private authService: AuthService) { }

  ngOnInit(): void {
    this.selectedStatus = Number(localStorage.getItem('statusId'));
  }

  statusChanged() {
    this.accountService.setStatus(this.selectedStatus).subscribe((data) => {
      localStorage.setItem('statusId', this.selectedStatus.toString());
      if (data) {
        Swal.fire({
          toast: true, position: 'top-end', showConfirmButton: false, timer: 1500, text: ' Status Changed to ' + this.status[this.selectedStatus - 1].status, icon: 'success', timerProgressBar: true
        })
      }
    })
  }

}
