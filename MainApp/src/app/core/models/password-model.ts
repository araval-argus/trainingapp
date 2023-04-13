export class PasswordModel{
  oldPassword?: string;
  newPassword?: string;
  confirmPassword?: string;
  username?: string;

  constructor(){
    this.username = "";
    this.oldPassword = "";
    this.newPassword = "";
  }
}
