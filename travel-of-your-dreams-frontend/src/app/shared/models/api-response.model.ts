export interface ApiResponse<T> {
  status: number;
  message: string;
  data: T;
}

export interface ApiListResponse<T> extends ApiResponse<T[]> {
  pagination: {
    page: number;
    limit: number;
    total: number;
    total_pages: number;
  };
}

export interface ApiErrorResponse {
  status: number;
  error: string;
  details: string[];
  timestamp: string;
  path: string;
}
