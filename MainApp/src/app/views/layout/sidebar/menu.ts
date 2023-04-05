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
    icon: 'home',
    link: '/profile/editProfile'
  },
  {
    label: 'View Profile',
    icon: 'home',
    link: '/profile/viewProfile'
  }
];
