import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { environment } from "src/environments/environment";


@Injectable({
  providedIn: 'root'
})

export class GroupService{

  UserRemoveGroupSub = new Subject<number>();
  GroupChangedSub = new Subject<number>();

  constructor(private http: HttpClient) { }

  createGroup( formdata : FormData ){
    return this.http.post(environment.apiUrl + "/Group/CreateGroup" , formdata);
  }

  loadRecentGroups(){
    return this.http.get(environment.apiUrl + "/Group/RecentGroup");
  }

  getAllMembers(groupId : number){
    return this.http.post(environment.apiUrl + "/Group/GetAllMembers", groupId);
  }

  getGroup(groupId : number){
    return this.http.post(environment.apiUrl + "/Group/GetGroup", groupId);
  }

  getAllUsers(groupId : number){
    return this.http.post(environment.apiUrl + "/Group/AllProfiles" , groupId);
  }

  addMemberToGroup(selUsers: string[],groupId:number){
    return this.http.post(environment.apiUrl + "/Group/AddMemberToGroup/" + groupId , selUsers);
  }

  editGroup(groupId:number,formdata:FormData){
    return this.http.post(environment.apiUrl + "/Group/UpdateGroup/" + groupId , formdata);
  }

  leaveGroup(groupId:number){
    return this.http.post(environment.apiUrl + "/Group/LeaveGroup" , groupId );
  }

  makeUserasAdmin(groupId:number,userName:string){
    return this.http.post(environment.apiUrl + "/Group/MakeAdmin/" + groupId , [userName]);
  }

  removeUserFromGroup(groupId:number,userName:string){
    return this.http.post(environment.apiUrl + "/Group/RemoveUser/" + groupId , [userName]);
  }

  sendFileMessage( formdata : FormData ){
    return this.http.post(environment.apiUrl + "/Group/SendFileMessage" , formdata);
  }

  fetchMessages(groupId:number){
    return this.http.post(environment.apiUrl + '/Group/GetAllMessage',groupId)
  }
}
