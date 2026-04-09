export type ItemType = 'TASK' | 'COMMITTEE';
<<<<<<< HEAD
=======
// Status is computed by the backend from DueDate and CompletedDate — never stored
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
export type ItemStatus = 'ACTIVE' | 'OVERDUE' | 'COMPLETED';
export type Importance = 'NORMAL' | 'SECRET';
export type CommitteeType = 'INTERNAL' | 'EXTERNAL';

export interface Item {
  id: number;
  itemNumber: string;
  type: ItemType;
  title: string;
  description: string;
  importance: Importance;
<<<<<<< HEAD
  committeeType?: CommitteeType | null;
  status: ItemStatus;
  completedDate?: string | null;
=======
  committeeType?: CommitteeType;
  status: ItemStatus;          // computed by backend
  completedDate?: string;      // ISO string, null if not completed
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
  dueDate: string;
  createdById: number;
  departmentId: number;
  createdAt: string;
  updatedAt: string;
  memberIds: number[];
  assigneeIds: number[];
  unreadCount?: number;
<<<<<<< HEAD
  hasUnreadUpdates?: boolean;
=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
}

export interface CreateItemRequest {
  type: ItemType;
  title: string;
  description: string;
  importance: Importance;
<<<<<<< HEAD
  committeeType?: CommitteeType | null;
  dueDate: string;
  memberIds: number[];
  assigneeIds: number[];
  memberNationalIds?: string[];
  assigneeNationalIds?: string[];
=======
  committeeType?: CommitteeType;
  dueDate: string;
  memberIds: number[];
  assigneeIds: number[];
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
}

export interface ChatMessage {
  id: number;
  itemId: number;
  userId: number;
  text: string;
<<<<<<< HEAD
  attachmentFileName?: string;
  hasAttachment?: boolean;
=======
  pdfAttachmentFileName?: string;
  pdfAttachmentPath?: string;
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
  createdAt: string;
  userName?: string;
}

<<<<<<< HEAD
export interface ItemAuditEvent {
  id: number;
  actionType: string;
  userId: number;
  userName: string;
  description: string;
  timestamp: string;
}

=======
>>>>>>> 69119a5b575ed698fed4fc8fa490e61e1e596f62
export interface Notification {
  id: number;
  userId: number;
  title: string;
  body: string;
  isRead: boolean;
  createdAt: string;
}
