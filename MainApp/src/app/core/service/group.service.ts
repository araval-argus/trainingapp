import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserModel } from '../models/UserModel';
import { GroupMessageModel } from '../models/group-message-model';
import { GroupModel } from '../models/group-model';
import { AuthService } from './auth-service';
import { GroupMemberModel } from '../models/group-member-model';

@Injectable({
  providedIn: 'root'
})
export class GroupService {

  constructor(private http: HttpClient,
    private authService: AuthService) { }

  groupSelected = new BehaviorSubject<GroupModel>(null);
  groupLeft = new Subject<GroupModel>();
  selectedGroupDetailsUpdated = new Subject<GroupModel>();

  createGroup(formData: FormData){
    return this.http.post(environment.apiUrl + "/group/CreateGroup", formData);
  }

  fetchGroups(userName: string){
    return this.http.get(environment.apiUrl + "/group/FetchGroups", {
      params: new HttpParams().append("userName", userName)
    });
  }

  fetchNotJoinedUsers(groupId: number){
    return this.http.get(environment.apiUrl + "/group/GetNotJoinedUsers", {
      params: new HttpParams()
      .append("groupId", groupId + "")
    })
  }

  fetchJoinedUsers(groupId: number){
    return this.http.get(environment.apiUrl + "/group/FetchJoinedMembers", {
      params: new HttpParams().append("groupId", groupId + "")
    } );
  }

  addMember(groupId: number, user: GroupMemberModel){
    return this.http.post(environment.apiUrl + "/group/AddMember", user,{
      params: new HttpParams().append("groupId", groupId+"")
    })
  }

  removeMember(memberUserName: string, groupId: number){
    const params = new HttpParams()
    .append("memberUserName", memberUserName)
    .append("groupId", groupId+"")
    return this.http.delete(environment.apiUrl + "/group/RemoveMember",{
      params: params
    })
  }

  updateGroup(groupModel: FormData){
    return this.http.post(environment.apiUrl + "/group/UpdateGroup", groupModel);
  }

  fetchMessages(memberUserName: string, groupId: number){
    return this.http.get(environment.apiUrl + "/group/FetchGroupMessages",{
      params: new HttpParams().append("userName", memberUserName).append("groupId", groupId+"")
    });
  }

  sendFile(formData: FormData){
    return this.http.post(environment.apiUrl + "/group/SendFileInGroup", formData);
  }

  makeAdmin(member: GroupMemberModel){
    return this.http.patch(environment.apiUrl + "/group/MakeAdmin", member);
  }

  fetchGroup(groupId: number){
    return this.http.get(environment.apiUrl + "/group/FetchGroup", {
      params: new HttpParams().append("groupId", groupId + "")
    });
  }
}
