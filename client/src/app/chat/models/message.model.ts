export interface Message {
  id: number,
  content: string,
  createdAt: Date,
  modifiedAt: Date | null
}
