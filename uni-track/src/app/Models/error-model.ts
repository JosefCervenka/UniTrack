export interface ValidationErrorState {
  httpStatus: number;
  field: string;
  reason: string;
}

export interface ApiValidationError {
  field: string;
  message: string;
  errorCode: number;
  state: ValidationErrorState | null;
}
