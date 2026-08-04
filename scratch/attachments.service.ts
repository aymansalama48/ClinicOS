import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../config/api-base-url.token';
import { Result } from '../../models/result.model';

export interface UploadedAttachment {
  attachmentId: string;
  fileName: string;
  contentType: string;
  size: number;
}

@Injectable({ providedIn: 'root' })
export class AttachmentsService {
  private http = inject(HttpClient);
  private baseUrl = inject(API_BASE_URL);

  uploadAttachment(file: File, entityType: string, entityId?: string): Observable<Result<UploadedAttachment>> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('entityType', entityType);
    if (entityId) {
      formData.append('entityId', entityId);
    }
    
    return this.http.post<Result<UploadedAttachment>>(`${this.baseUrl}/api/attachments/upload`, formData);
  }

  getDownloadUrl(attachmentId: string): string {
    return `${this.baseUrl}/api/attachments/${attachmentId}/download`;
  }
}
