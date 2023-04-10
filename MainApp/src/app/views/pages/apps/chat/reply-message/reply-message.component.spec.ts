import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReplyMessageComponent } from './reply-message.component';

describe('ReplyMessageComponent', () => {
  let component: ReplyMessageComponent;
  let fixture: ComponentFixture<ReplyMessageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReplyMessageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReplyMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
