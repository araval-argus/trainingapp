import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatContentFooterComponent } from './chat-content-footer.component';

describe('ChatContentFooterComponent', () => {
  let component: ChatContentFooterComponent;
  let fixture: ComponentFixture<ChatContentFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChatContentFooterComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatContentFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
