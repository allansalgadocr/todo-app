export interface TodoItemCreateDto {
  title: string;
  description?: string;
}

export interface TodoItemReadDto {
  id: string; // UUID
  title: string | null;
  description: string | null;
  isCompleted: boolean;
  createdAt: string; // ISO 8601 date-time string
}

export interface TodoItemUpdateDto {
  title: string;
  description?: string;
  isCompleted?: boolean;
}
