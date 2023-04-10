import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatBaseHeaderComponent } from './chat-content-header.component';

describe('ChatBaseHeaderComponent', () => {
  let component: ChatBaseHeaderComponent;
  let fixture: ComponentFixture<ChatBaseHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChatBaseHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatBaseHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
