import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ColleagueModel } from 'src/app/core/models/colleague-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss']
})
export class EmployeesComponent implements OnInit {

  loggedInUser:LoggedInUser;
  users : ColleagueModel[];
  selUser : ColleagueModel;
  desigList : {id:number,position:number}[];
  environment = environment.ImageUrl;

  constructor(private accountService : AccountService , private authService:AuthService , private modalService: NgbModal) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();

    this.accountService.getAllUsers().subscribe((data:ColleagueModel[])=>{
      this.users = data;
    });
  }

  onEdit(user:ColleagueModel){
    this.selUser = {...user};
    this.accountService.getAllDesignation().subscribe((data:any)=>{
      this.desigList = data;
    })
  }

  onDelete(user:ColleagueModel){
    Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, Remove '+ user.firstName + user.lastName + ' !'
    }).then((result) => {
      if (result.isConfirmed) {
        this.accountService.DeleteUser(user.userName).subscribe(()=>{});
        this.users = this.users.filter(u => u.userName !== user.userName);
        Swal.fire(
          'Removed!',
          user.firstName + user.lastName + ' is Deleted',
          'success'
        )
      }
    });
  }

  onUpdate(userName:string){
    const formdata = new FormData;
    formdata.append('userName',this.selUser.userName);
    formdata.append('firstName',this.selUser.firstName);
    formdata.append('lastName',this.selUser.lastName);
    formdata.append('email',this.selUser.email);
    formdata.append('designation',this.selUser.designation);
    this.accountService.updateByAdmin(formdata).subscribe(()=>{
    });
    const index = this.users.findIndex(user => user.userName === userName);
    if (index !== -1) {
      this.users[index].firstName = this.selUser.firstName;
      this.users[index].lastName = this.selUser.lastName;
      this.users[index].email = this.selUser.email;
      this.users[index].designation = this.selUser.designation;
    }
  }

  openBasicModal(content) {
    this.modalService.open(content, {}).result.then((result) => {
    }).catch((res) => {});
  }

}
