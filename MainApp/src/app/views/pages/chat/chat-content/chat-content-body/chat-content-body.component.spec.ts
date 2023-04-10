import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatContentBodyComponent } from './chat-content-body.component';

describe('ChatContentBodyComponent', () => {
  let component: ChatContentBodyComponent;
  let fixture: ComponentFixture<ChatContentBodyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChatContentBodyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatContentBodyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
