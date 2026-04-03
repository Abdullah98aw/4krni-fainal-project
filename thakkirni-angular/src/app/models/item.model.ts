export type ItemType = 'TASK' | 'COMMITTEE';
// Status is computed by the backend from DueDate and CompletedDate — never stored
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
  committeeType?: CommitteeType;
  status: ItemStatus;          // computed by backend
  completedDate?: string;      // ISO string, null if not completed
  dueDate: string;
  createdById: number;
  departmentId: number;
  createdAt: string;
  updatedAt: string;
  memberIds: number[];
  assigneeIds: number[];
  unreadCount?: number;
}

export interface CreateItemRequest {
  type: ItemType;
  title: string;
  description: string;
  importance: Importance;
  committeeType?: CommitteeType;
  dueDate: string;
  memberIds: number[];
  assigneeIds: number[];
}

export interface ChatMessage {
  id: number;
  itemId: number;
  userId: number;
  text: string;
  pdfAttachmentFileName?: string;
  pdfAttachmentPath?: string;
  createdAt: string;
  userName?: string;
}

export interface Notification {
  id: number;
  userId: number;
  title: string;
  body: string;
  isRead: boolean;
  createdAt: string;
}
