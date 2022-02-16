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
    label: 'Account',
    isTitle: true
  },
  {
    label: 'Profile',
    icon: 'user',
    link: '/account/profile'
  },
  {
    label: 'Edit Profile',
    icon: 'edit',
    link: '/account/edit'
  }
];
