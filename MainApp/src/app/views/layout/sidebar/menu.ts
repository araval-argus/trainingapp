import { MenuItem } from './menu.model';

export const MENU: MenuItem[] = [
  {
    label: 'Main',
    isTitle: true
  },
  {
    label: 'Dashboard',
    icon: 'home',
    link: '/dashboard'
  },
  {
    label: 'Profile',
    isTitle: true
  },
  {
    label: 'Edit Profile',
    icon: 'edit-2',
    link: '/profile/editProfile'
  },
  {
    label: 'View Profile',
    icon: 'eye',
    link: '/profile/viewProfile'
  },
  {
    label: 'Chat',
    isTitle: true
  },
  {
    label: 'Messages',
    icon: 'inbox',
    link: '/chat'
  }
];
