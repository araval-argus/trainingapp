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
    label: 'Chat',
    icon: 'users',
    link: '/chat'
  },
  {
    label:'Profile',
    isTitle:true
  },
  {
    label: 'View Profile',
    icon: 'user-check',
    link: 'view-profile'
  },
  {
    label: 'Edit Profile',
    icon: 'edit',
    link: 'update-profile'
  }
];
