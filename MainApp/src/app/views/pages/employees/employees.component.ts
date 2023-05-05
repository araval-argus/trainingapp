import { Component, OnInit } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { DesignationModel } from "src/app/core/models/designation-model";
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { UserModel } from "src/app/core/models/UserModel";
import { AccountService } from "src/app/core/service/account-service";
import { AdminService } from "src/app/core/service/admin-service";
import { AuthService } from 'src/app/core/service/auth-service';
import { environment } from "src/environments/environment";
import Swal from 'sweetalert2';

@Component({
  selector: "app-employees",
  templateUrl: "./employees.component.html",
  styleUrls: ["./employees.component.scss"],
})
export class EmployeesComponent implements OnInit {
  employees: UserModel[] = [];
  isAdmin: Boolean = false;
  selectedEmployee: UserModel;
  //for checking the username exists or not
  selectedEmployeeUserName: string;
  loggedInUser: LoggedInUserModel;

  designations: DesignationModel[];

  usernameExists: boolean = false;
  emailExists: boolean = false;

  constructor(
    private adminService: AdminService,
    private modalService: NgbModal,
    private accountService: AccountService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.fetchEmployees();
    this.checkIsAdmin();
  }

  checkIsAdmin() {
    this.isAdmin =
      this.loggedInUser.designation === "Chief Technical Officer" ||
      this.loggedInUser.designation === "Chief Executive Officer";
  }

  fetchEmployees() {
    this.adminService.fetchEmployees().subscribe((data: UserModel[]) => {
      this.employees = data;
      this.employees.forEach((employee) => {
        employee.imageUrl =
          environment.apiUrl + "/../Images/Users/" + employee.imageUrl;
      });
    });
  }

  deleteEmployee(employee: UserModel) {
    Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
      if (result.isConfirmed) {
        Swal.fire(
          'Deleted!',
          'User Deleted',
          'success'
        )
        this.employees.splice(this.employees.indexOf(employee), 1);
        this.adminService.deleteEmployee(employee.userName).subscribe();
      }
    })

  }

  selectEmployee(employee: UserModel) {
    this.selectedEmployee = { ...employee };
    this.selectedEmployeeUserName = this.selectedEmployee.userName;
    console.log("selected employee", employee)
  }

  openScrollableModal(content) {
    this.accountService.fetchDesignations().subscribe((data: any) => {
      this.designations = data.designations;
      this.modalService
      .open(content, { scrollable: true })
      .result.then((result) => {})
      .catch((res) => {});
    });

  }

  //update
  updateEmployeeData() {
    //console.log("updated employee data", this.selectedEmployee)
    this.adminService
      .updateEmployeeData(this.selectedEmployee, this.selectedEmployeeUserName)
      .subscribe(() => {
        this.fetchEmployees();
      });
  }

  //for checking unique username
  onUserNameValueChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;


      //dont find if both are equal
      if (value !== this.selectedEmployeeUserName) {
        const employee = this.employees.find(
          (employee) => employee.userName === value
        );
        employee ? (this.usernameExists = true) : (this.usernameExists = false);
      }
  }

  //to update the value of dropdown of designations
  designationChanged(event) {
    this.selectedEmployee.designation =
      this.designations[event.target.selectedIndex];
  }

  onEmailCanged(event: Event) {
    const value = (event.target as HTMLInputElement).value;

    const employee = this.employees.find(
      (employee) => employee.email === value
    );

    employee ? (this.emailExists = true) : (this.emailExists = false);
  }
}
