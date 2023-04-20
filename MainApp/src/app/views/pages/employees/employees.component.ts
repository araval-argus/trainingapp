import { Component, OnInit } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { DesignationModel } from "src/app/core/models/designation-model";
import { FriendProfileModel } from "src/app/core/models/friend-profile-model";
import { AccountService } from "src/app/core/service/account-service";
import { AdminService } from "src/app/core/service/admin-service";
import { AuthService } from 'src/app/core/service/auth-service';
import { environment } from "src/environments/environment";

@Component({
  selector: "app-employees",
  templateUrl: "./employees.component.html",
  styleUrls: ["./employees.component.scss"],
})
export class EmployeesComponent implements OnInit {
  employees: FriendProfileModel[] = [];
  isAdmin: Boolean = false;
  selectedEmployee: FriendProfileModel;
  //for checking the username exists or not
  selectedEmployeeUserName: string;

  designations: DesignationModel[];
  timeOutIdForUsername;

  usernameExists: boolean = false;

  constructor(
    private adminService: AdminService,
    private modalService: NgbModal,
    private accountService: AccountService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.fetchEmployees();
    this.checkIsAdmin();
  }

  checkIsAdmin(){
    this.isAdmin = this.authService.getLoggedInUserInfo().designation === "Chief Technical Officer";
  }

  fetchEmployees(){
     this.adminService
      .fetchEmployees()
      .subscribe((data: FriendProfileModel[]) => {
        this.employees = data;
        this.employees.forEach((employee) => {
          employee.imageUrl =
            environment.apiUrl + "/../Images/Users/" + employee.imageUrl;
        });
      });
  }

  deleteEmployee(employee: FriendProfileModel) {
    this.employees.splice(this.employees.indexOf(employee), 1);

    this.adminService.deleteEmployee(employee.userName).subscribe();
  }

  selectEmployee(employee: FriendProfileModel) {
    this.selectedEmployee = {...employee};
    this.selectedEmployeeUserName = this.selectedEmployee.userName;
  }

  openScrollableModal(content) {
    this.accountService.fetchDesignations().subscribe((data: any) => {
      this.designations = data.designations;
    });
    this.modalService
      .open(content, { scrollable: true })
      .result.then((result) => {
      })
      .catch((res) => {});
  }

  //update
  updateEmployeeData() {
    this.adminService.updateEmployeeData(this.selectedEmployee).subscribe(() => {
      this.fetchEmployees()
    });
  }

  //for checking unique username
  onUserNameValueChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    if (this.timeOutIdForUsername) {
      clearTimeout(this.timeOutIdForUsername);
    }

    this.timeOutIdForUsername = setTimeout(() => {

      //dont send a request if both are equal
      if (value !== this.selectedEmployeeUserName) {
        this.accountService.checkUsername(value).subscribe((data) => {
          console.log(data.usernameExists);
          this.usernameExists = data.usernameExists;
        });
      }
    }, 1000);
  }

  //to update the value of dropdown of designations
  designationChanged(event) {
    this.selectedEmployee.designation = this.designations[event.target.selectedIndex];
  }
}
