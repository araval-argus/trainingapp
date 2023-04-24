import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient } from "@angular/common/http";
import { Subject } from "rxjs";
import { GroupModel } from "../models/group-model";
import { GroupNewChat } from "../models/groupNewChat-model";

@Injectable({
    providedIn: "root"
})
export class GroupService {

    public readonly groupSelection = new Subject<GroupModel>();
    public readonly openFromNotification = new Subject<string>();

    constructor(private http: HttpClient) { }
    addGroup(formData: FormData) {
        return this.http.post(environment.apiUrl + "/group/addGroup", formData)
    }

    getAllGroup() {
        return this.http.get(environment.apiUrl + "/group/getAll");
    }
    getMembers(groupName: string) {
        return this.http.get(environment.apiUrl + "/group/getMembers?name=" + groupName);
    }
    addMembers(membersList: string[], groupName: string) {
        return this.http.post(environment.apiUrl + "/group/addMembers?groupName=" + groupName, membersList)
    }
    addMessage(newChat: GroupNewChat) {
        return this.http.post(environment.apiUrl + "/group/addMessage", newChat)
    }

    getAllChat(groupName: string) {
        return this.http.get(environment.apiUrl + "/group/getAllChat?groupName=" + groupName);
    }

    fetchImage(name: string) {
        return environment.api + "/images/" + name;
    }
    addFile(formData: FormData) {
        return this.http.post(environment.apiUrl + "/group/addFile", formData);
    }
    updateGroup(formData: FormData) {
        return this.http.post(environment.apiUrl + "/group/updateGroup", formData);
    }
    removeMember(removeList: string[], groupName: string) {
        return this.http.post(environment.apiUrl + "/group/removeMember?groupName=" + groupName, removeList);
    }
    leaveGroup(groupName: string) {
        return this.http.get(environment.apiUrl + "/group/leaveGroup?groupName=" + groupName)
    }
}