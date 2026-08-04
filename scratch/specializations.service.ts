import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../../config/api-base-url.token';
import { Result } from '../../models/result.model';
import { PagedResult, PaginationParameters } from '../../models/pagination.model';
import { Specialization, SpecializationSchedule } from '../../models/specialization.model';

@Injectable({ providedIn: 'root' })
export class SpecializationsService {
  private http = inject(HttpClient);
  private baseUrl = inject(API_BASE_URL);

  getSpecializations(params?: PaginationParameters & { searchTerm?: string }): Observable<Result<PagedResult<Specialization>>> {
    let httpParams = new HttpParams();
    if (params?.pageNumber) httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    if (params?.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params?.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);

    return this.http.get<Result<PagedResult<Specialization>>>(`${this.baseUrl}/api/specializations`, { params: httpParams });
  }

  getLookup(searchTerm?: string): Observable<Result<Specialization[]>> {
    let httpParams = new HttpParams();
    if (searchTerm) httpParams = httpParams.set('searchTerm', searchTerm);
    return this.http.get<Result<Specialization[]>>(`${this.baseUrl}/api/specializations/lookup`, { params: httpParams });
  }

  getSpecializationById(id: string): Observable<Result<Specialization>> {
    return this.http.get<Result<Specialization>>(`${this.baseUrl}/api/specializations/${id}`);
  }

  createSpecialization(name: string, description?: string, iconAttachmentId?: string): Observable<Result<string>> {
    return this.http.post<Result<string>>(`${this.baseUrl}/api/specializations`, { name, description, iconAttachmentId });
  }

  updateSpecialization(id: string, name: string, description?: string, iconAttachmentId?: string): Observable<Result> {
    return this.http.put<Result>(`${this.baseUrl}/api/specializations/${id}`, { name, description, iconAttachmentId });
  }

  deleteSpecialization(id: string): Observable<Result> {
    return this.http.delete<Result>(`${this.baseUrl}/api/specializations/${id}`);
  }

  toggleStatus(id: string): Observable<Result> {
    return this.http.patch<Result>(`${this.baseUrl}/api/specializations/${id}/toggle-status`, {});
  }

  addSchedule(specializationId: string, schedule: Partial<SpecializationSchedule>): Observable<Result<string>> {
    return this.http.post<Result<string>>(`${this.baseUrl}/api/specializations/${specializationId}/schedules`, schedule);
  }

  updateSchedule(specializationId: string, scheduleId: string, schedule: Partial<SpecializationSchedule>): Observable<Result> {
    return this.http.put<Result>(`${this.baseUrl}/api/specializations/${specializationId}/schedules/${scheduleId}`, schedule);
  }

  removeSchedule(specializationId: string, scheduleId: string): Observable<Result> {
    return this.http.delete<Result>(`${this.baseUrl}/api/specializations/${specializationId}/schedules/${scheduleId}`);
  }

  toggleScheduleStatus(specializationId: string, scheduleId: string): Observable<Result> {
    return this.http.patch<Result>(`${this.baseUrl}/api/specializations/${specializationId}/schedules/${scheduleId}/toggle-status`, {});
  }
}
