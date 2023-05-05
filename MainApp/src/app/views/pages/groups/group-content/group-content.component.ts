import { Component, Input, OnInit } from '@angular/core';
import { GroupModel } from 'src/app/core/models/group-model';
import { GroupMemberModel } from 'src/app/core/models/group-member-model';

@Component({
  selector: "app-group-content",
  templateUrl: "./group-content.component.html",
  styleUrls: ["./group-content.component.scss"],
})
export class GroupContentComponent implements OnInit {
  @Input() selectedGroup: GroupModel;
  @Input() joinedUsers: GroupMemberModel[];

  constructor() {}

  ngOnInit(): void {
  }
}
