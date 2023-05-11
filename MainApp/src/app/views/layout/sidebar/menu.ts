import { MenuItem } from './menu.model';

export const MENU: MenuItem[] = [
  {
    label: 'Main',
    isTitle: true
  },
  {
    label: 'Dashboard',
    icon: 'home',
    link: '/dashboard',
  },
  {
    label: 'Employees',
    icon: 'list',
    link: 'employees'
  },
  {
    label: 'Chat',
    icon: 'user',
    link: '/chat'
  },
  {
    label: 'Group',
    icon : 'users',
    link : '/group'
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
