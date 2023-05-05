import { Component, OnInit, TemplateRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationService } from 'primeng/api';
import { Profile } from 'src/app/core/models/profile-model';
import { RegistrationModel } from 'src/app/core/models/registration-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { EmployeeService } from 'src/app/core/service/employee-service';
import { HubService } from 'src/app/core/service/hub-service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.scss'],
})
export class EmployeeComponent implements OnInit {
  values: string[];
  selectedValue: string = 'val1';

  selectedEmployee: Profile;

  employees: Profile[] = [];

  cols: any[];

  statuses: any[];

  profileTypes: any[];

  display: boolean = false;
  statusCode: string[] = [
    "Available", "Busy", "Do Not Disturb", "Be Right Back", "Appear Away", "Appear Offline"
  ];


  profileTypeName: string[] = [
    "Admin", "CEO", "CTO", "Group Lead", "Solution Analyst", "Programme Analyst", "Probatitioner", "Intern"
  ];

  availableProfileType: string[] = [
    "Group Lead", "Solution Analyst", "Programme Analyst", "Probatitioner", "Intern"
  ];



  editing: boolean = false;

  //temp var
  selectedProfile;

  //boolean to check role
  isAdmin: boolean = false;

  //boolean for new user dialogue
  newUserDialog: boolean = false;

  //to manage new member 
  regModel: RegistrationModel;
  disableRegButtton: boolean = false;


  constructor(private employeeService: EmployeeService, private accountService: AccountService, private authService: AuthService, private confirmationService: ConfirmationService, private modalService: NgbModal, private hubService: HubService) {
  }

  ngOnInit(): void {


    const desingnation = this.authService.getLoggedInUserInfo().desingnation;
    if (desingnation == "Admin" || desingnation == "CEO" || desingnation == "CTO") {
      this.isAdmin = true;
    }

    this.employeeService.getAllEmployee().subscribe((data: Profile[]) => {
      this.employees = data;
    })

    this.cols = [
      { field: 'firstName', header: 'First Name' },
      { field: 'lastName', header: 'Last Name' },
      { field: 'userName', header: 'UserName' },
      { field: 'email', header: 'Email' },
      { field: 'profileType', header: 'Profile Type' },
      { field: 'status', header: 'Status' }
    ];


    this.statuses = [
      { label: "Available", value: 1 },
      { label: "Busy", value: 2 },
      { label: "Do Not Disturb", value: 3 },
      { label: "Be Right Back", value: 4 },
      { label: "Appear Away", value: 5 },
      { label: "Appear Offline", value: 6 }
    ];

    this.profileTypes = [
      { label: "Admin", value: 1 },
      { label: "CEO", value: 2 },
      { label: "CTO", value: 3 },
      { label: "Group Lead", value: 4 },
      { label: "Solution Analyst", value: 5 },
      { label: "Programme Analyst", value: 6 },
      { label: "Probatitioner", value: 7 },
      { label: "Intern", value: 8 }
    ];

    this.resetForm();


    this.hubService.hubConnection.on("userUpdated", (userName, userProfileType) => {
      const profile: Profile = this.employees.find(e => e.userName == userName);
      profile.profileType = userProfileType;
    })


    this.hubService.hubConnection.on("userStatusUpdated", (userName: string, id: number) => {
      const profile: Profile = this.employees.find(e => e.userName == userName);
      profile.status = id;
    })


    this.hubService.hubConnection.on("newUserAdded", (profile: Profile) => {
      this.employees.push(profile);
    })
  }


  fetchImage(imageName: string) {
    if (imageName == null) {
      return "https://via.placeholder.com/32x32"
    }
    return this.accountService.fetchImage(imageName);
  }

  roleChanged(event: string, user: Profile) {
    const indexOfProfile = this.profileTypes.find(e => e.label == event).value;
    const Profile = this.employees.find(e => e.userName == user.userName);
    const temp = Profile.profileType;
    console.log(Profile);
    this.confirmationService.confirm({
      message: 'Are you sure that you want to change ' + user.userName + "'s role from " + this.profileTypeName[user.profileType - 1] + ' to ' + event + "?",
      accept: () => {
        this.employeeService.updateRole(user.userName, indexOfProfile).subscribe(data => {
          if (data) {
            Profile.profileType = indexOfProfile;
            Swal.fire({
              toast: true, position: 'top-end', showConfirmButton: false, timer: 3000, text: user.userName + "'s role changed from " + this.profileTypeName[user.profileType - 1] + ' to ' + event, icon: 'success', timerProgressBar: true
            })
          } else {
            Swal.fire({
              toast: true, position: 'top-end', showConfirmButton: false, timer: 3000, text: 'Something Went Wrong', icon: 'error', timerProgressBar: true
            })
          }
        })
      },
      reject: () => {
        Profile.profileType = temp;
        console.log(Profile);
      }
    });
  }

  deleteEmployee(employee: Profile) {
    this.confirmationService.confirm({
      message: 'Are you sure that you want to delete' + employee.userName + "'s profile?",
      accept: () => {
        this.employeeService.deleteUser(employee.userName).subscribe(data => {
          if (data) {
            this.employees = this.employees.filter(e => e !== employee);
            Swal.fire({
              toast: true, position: 'top-end', showConfirmButton: false, timer: 3000, text: employee.userName + " deleted Successfully", icon: 'success', timerProgressBar: true
            })
          } else {
            Swal.fire({
              toast: true, position: 'top-end', showConfirmButton: false, timer: 3000, text: 'Something Went Wrong', icon: 'error', timerProgressBar: true
            })
          }
        })
      }
    })
  }


  addNewUser(content: TemplateRef<any>) {
    this.modalService.open(content);
  }

  onRegister(event: Event) {
    event.preventDefault();
    this.regModel.password = this.regModel.email;
    this.regModel.type = this.profileTypes.find(e => e.label == this.regModel.type).value;
    this.disableRegButtton = true;
    this.employeeService.addUser(this.regModel)
      .subscribe((data: any) => {
        Swal.fire({
          toast: true, position: 'top-end', showConfirmButton: false, timer: 3000, text: 'User Created Successfully', icon: 'success', timerProgressBar: true
        })
        this.resetForm();
        this.modalService.dismissAll();
      }, (err) => {
        this.disableRegButtton = false;
        Swal.fire({
          title: 'Error!',
          text: err.error.message,
          icon: 'error',
        });
      });
  }

  resetForm() {
    this.regModel = {
      firstName: '',
      lastName: '',
      userName: '',
      email: '',
      password: '',
      type: 8
    }
  }
}
