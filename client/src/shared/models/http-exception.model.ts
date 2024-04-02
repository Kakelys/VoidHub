export interface HttpException {
  statusCode: number;
  errors: string[];
  error: any;
}
