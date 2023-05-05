import { MenuItem } from "./menu.model";

export const MENU: MenuItem[] = [
  {
    label: "Main",
    isTitle: true,
  },
  {
    label: "Dashboard",
    icon: "home",
    link: "/dashboard",
  },
  {
    label: "Chats",
    icon: "inbox",
    link: "/chats",
  },
  {
    label: "Groups",
    icon: "users",
    link: "/groups",
  }
];
