import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { SpecializationsService, Specialization, AttachmentsService, SpecializationSchedule, DayOfWeek, PeriodType } from '@clinicos/core';
import { ButtonComponent, CardComponent, EmptyStateComponent, InputComponent } from '@clinicos/shared';
import { firstValueFrom } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-specializations-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ButtonComponent, CardComponent, EmptyStateComponent, InputComponent],
  template: `
    <div class="space-y-6 animate-in fade-in zoom-in duration-300" dir="rtl">
      <!-- Header -->
      <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
        <div>
          <h1 class="text-3xl font-bold tracking-tight text-[var(--text)]">التخصصات</h1>
          <p class="text-[var(--text-muted)] mt-1">إدارة التخصصات الطبية للعيادة والمواعيد المتاحة.</p>
        </div>
        <sh-button variant="primary" (clicked)="openModal()">
          <i class="fa-solid fa-plus ml-2 rtl:ml-2 rtl:mr-0"></i> إضافة تخصص
        </sh-button>
      </div>

      <!-- Controls & Table -->
      <sh-card>
        <div class="mb-6 flex items-center justify-between gap-4">
          <div class="w-full max-w-sm">
            <sh-input
              id="search"
              placeholder="ابحث عن التخصصات..."
              icon="fa-solid fa-search"
              [(ngModel)]="searchTerm"
              (ngModelChange)="onSearch()"
            ></sh-input>
          </div>
        </div>

        @if (isLoading()) {
          <div class="flex justify-center p-12">
            <svg class="animate-spin h-8 w-8 text-[var(--color-brand-600)]" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
          </div>
        } @else if (specializations().length === 0) {
          <sh-empty-state
            icon="fa-solid fa-list"
            title="لم يتم العثور على تخصصات"
            description="لم نتمكن من العثور على أي تخصصات تطابق بحثك."
          ></sh-empty-state>
        } @else {
          <div class="overflow-x-auto rounded-[var(--radius-md)] border border-[var(--border)]">
            <table class="w-full text-sm text-start text-[var(--text)]">
              <thead class="text-xs uppercase bg-[var(--bg-muted)] border-b border-[var(--border)] text-[var(--text-muted)]">
                <tr>
                  <th scope="col" class="px-6 py-4 font-semibold text-start">الأيقونة</th>
                  <th scope="col" class="px-6 py-4 font-semibold text-start">اسم التخصص</th>
                  <th scope="col" class="px-6 py-4 font-semibold text-start">الوصف</th>
                  <th scope="col" class="px-6 py-4 font-semibold text-start">الحالة</th>
                  <th scope="col" class="px-6 py-4 font-semibold text-end">الإجراءات</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-[var(--border)]">
                @for (spec of specializations(); track spec.id) {
                  <tr class="bg-[var(--bg-surface)] hover:bg-[var(--bg-app)] transition-colors">
                    <td class="px-6 py-4">
                      @if (spec.iconAttachmentId) {
                        <img [src]="getIconUrl(spec.iconAttachmentId)" alt="{{ spec.name }}" class="w-10 h-10 rounded-full object-cover shadow-sm border border-[var(--border)]">
                      } @else {
                        <div class="w-10 h-10 rounded-full bg-[var(--bg-muted)] flex items-center justify-center text-[var(--text-muted)]">
                          <i class="fa-solid fa-stethoscope"></i>
                        </div>
                      }
                    </td>
                    <td class="px-6 py-4 font-medium">{{ spec.name }}</td>
                    <td class="px-6 py-4 text-[var(--text-muted)]">{{ spec.description || '-' }}</td>
                    <td class="px-6 py-4">
                      <button (click)="toggleStatus(spec.id)" class="px-2 py-1 rounded-full text-xs font-medium transition-colors border"
                              [class]="spec.isActive ? 'bg-emerald-50 text-emerald-600 border-emerald-200 hover:bg-emerald-100' : 'bg-red-50 text-red-600 border-red-200 hover:bg-red-100'">
                        {{ spec.isActive ? 'نشط' : 'غير نشط' }}
                      </button>
                    </td>
                    <td class="px-6 py-4 text-end">
                      <div class="flex items-center justify-end gap-2">
                        <sh-button variant="outline" size="sm" (clicked)="openSchedulesModal(spec)">
                          <i class="fa-solid fa-calendar-alt ml-1"></i> المواعيد
                        </sh-button>
                        <sh-button variant="ghost" size="icon" (clicked)="openModal(spec)">
                          <i class="fa-solid fa-pen text-[var(--text-muted)] hover:text-[var(--color-brand-600)]"></i>
                        </sh-button>
                        <sh-button variant="ghost" size="icon" (clicked)="deleteSpec(spec.id)">
                          <i class="fa-solid fa-trash text-[var(--text-muted)] hover:text-[var(--color-danger)]"></i>
                        </sh-button>
                      </div>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
          
          <div class="flex flex-col sm:flex-row items-center justify-between mt-4 gap-4 text-sm text-[var(--text-muted)]">
            <div>صفحة {{ currentPage() }} من {{ totalPages() || 1 }}</div>
            <div class="flex gap-2">
              <sh-button variant="outline" size="sm" [disabled]="!hasPrev()" (clicked)="changePage(-1)">السابق</sh-button>
              <sh-button variant="outline" size="sm" [disabled]="!hasNext()" (clicked)="changePage(1)">التالي</sh-button>
            </div>
          </div>
        }
      </sh-card>
    </div>

    <!-- Specialization Modal -->
    @if (showModal()) {
      <div class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm animate-in fade-in duration-200" dir="rtl">
        <sh-card customClass="w-full max-w-lg mx-auto shadow-2xl animate-in zoom-in-95 duration-200">
          <div class="flex items-center justify-between border-b border-[var(--border)] pb-4 mb-4">
            <h3 class="text-xl font-bold text-[var(--text)]">{{ isEditing() ? 'تعديل تخصص' : 'إضافة تخصص جديد' }}</h3>
            <button class="text-[var(--text-muted)] hover:text-[var(--text)]" (click)="closeModal()">
              <i class="fa-solid fa-times text-xl"></i>
            </button>
          </div>

          <form [formGroup]="specForm" (ngSubmit)="onSubmit()" class="space-y-4">
            <sh-input
              id="name"
              label="اسم التخصص"
              placeholder="مثال: طب الأطفال"
              formControlName="name"
              [required]="true"
              [error]="specForm.get('name')?.touched && specForm.get('name')?.invalid ? 'هذا الحقل مطلوب' : ''"
            ></sh-input>

            <sh-input
              id="description"
              label="الوصف (اختياري)"
              placeholder="وصف مختصر للتخصص..."
              formControlName="description"
            ></sh-input>

            <div class="space-y-2">
              <label class="block text-sm font-medium text-[var(--text)]">الأيقونة أو صورة التخصص</label>
              
              @if (iconPreviewUrl()) {
                <div class="relative w-24 h-24 mb-2">
                  <img [src]="iconPreviewUrl()" class="w-full h-full object-cover rounded-xl border border-[var(--border)]">
                  <button type="button" (click)="clearFile()" class="absolute -top-2 -right-2 bg-red-100 text-red-600 rounded-full w-6 h-6 flex items-center justify-center hover:bg-red-200">
                    <i class="fa-solid fa-times text-xs"></i>
                  </button>
                </div>
              }
              
              <input type="file" accept="image/*" (change)="onFileSelected($event)" class="block w-full text-sm text-[var(--text-muted)] file:mr-4 file:py-2 file:px-4 file:rounded-full file:border-0 file:text-sm file:font-semibold file:bg-[var(--color-brand-50)] file:text-[var(--color-brand-700)] hover:file:bg-[var(--color-brand-100)]"/>
            </div>

            @if (apiError()) {
              <div class="p-3 text-sm text-[var(--color-danger)] bg-red-50 border border-red-100 rounded-lg">
                {{ apiError() }}
              </div>
            }

            <div class="flex justify-end gap-3 pt-4 border-t border-[var(--border)]">
              <sh-button variant="ghost" type="button" (clicked)="closeModal()">إلغاء</sh-button>
              <sh-button variant="primary" type="submit" [disabled]="specForm.invalid || isSaving()" [loading]="isSaving()">
                {{ isEditing() ? 'حفظ التعديلات' : 'إضافة' }}
              </sh-button>
            </div>
          </form>
        </sh-card>
      </div>
    }

    <!-- Schedules Modal -->
    @if (showSchedulesModal()) {
      <div class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm animate-in fade-in duration-200" dir="rtl">
        <sh-card customClass="w-full max-w-3xl mx-auto shadow-2xl animate-in zoom-in-95 duration-200 max-h-[90vh] overflow-y-auto">
          <div class="flex items-center justify-between border-b border-[var(--border)] pb-4 mb-4">
            <div>
              <h3 class="text-xl font-bold text-[var(--text)]">مواعيد التخصص: {{ selectedSpecForSchedules()?.name }}</h3>
              <p class="text-[var(--text-muted)] text-sm">إدارة أيام وساعات العمل لهذا التخصص.</p>
            </div>
            <button class="text-[var(--text-muted)] hover:text-[var(--text)]" (click)="closeSchedulesModal()">
              <i class="fa-solid fa-times text-xl"></i>
            </button>
          </div>

          <!-- Add Schedule Form -->
          <form [formGroup]="scheduleForm" (ngSubmit)="onAddSchedule()" class="bg-[var(--bg-muted)] p-4 rounded-lg mb-6 border border-[var(--border)]">
            <h4 class="font-semibold mb-3 text-sm">إضافة موعد جديد</h4>
            <div class="grid grid-cols-1 sm:grid-cols-4 gap-4 items-end">
              <div>
                <label class="block text-sm font-medium mb-1">اليوم</label>
                <select formControlName="dayOfWeek" class="w-full bg-[var(--bg-surface)] border border-[var(--border)] rounded-md px-3 py-2 text-sm">
                  <option [value]="0">الأحد</option>
                  <option [value]="1">الإثنين</option>
                  <option [value]="2">الثلاثاء</option>
                  <option [value]="3">الأربعاء</option>
                  <option [value]="4">الخميس</option>
                  <option [value]="5">الجمعة</option>
                  <option [value]="6">السبت</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium mb-1">الفترة</label>
                <select formControlName="period" class="w-full bg-[var(--bg-surface)] border border-[var(--border)] rounded-md px-3 py-2 text-sm">
                  <option [value]="1">صباحية</option>
                  <option [value]="2">مسائية</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium mb-1">من الساعة</label>
                <input type="time" formControlName="startTime" class="w-full bg-[var(--bg-surface)] border border-[var(--border)] rounded-md px-3 py-2 text-sm" />
              </div>
              <div>
                <label class="block text-sm font-medium mb-1">إلى الساعة</label>
                <input type="time" formControlName="endTime" class="w-full bg-[var(--bg-surface)] border border-[var(--border)] rounded-md px-3 py-2 text-sm" />
              </div>
            </div>
            <div class="mt-4 flex justify-end">
              <sh-button variant="primary" type="submit" [disabled]="scheduleForm.invalid" [loading]="isSavingSchedule()">
                <i class="fa-solid fa-plus ml-1"></i> إضافة الموعد
              </sh-button>
            </div>
          </form>

          <!-- Schedules List -->
          @if (isLoadingSchedules()) {
            <div class="flex justify-center p-8">
              <svg class="animate-spin h-6 w-6 text-[var(--color-brand-600)]" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path></svg>
            </div>
          } @else if (specSchedules().length === 0) {
            <div class="text-center p-6 text-[var(--text-muted)] border border-dashed border-[var(--border)] rounded-lg">
              لا توجد مواعيد مضافة لهذا التخصص.
            </div>
          } @else {
            <div class="overflow-x-auto rounded-[var(--radius-md)] border border-[var(--border)]">
              <table class="w-full text-sm text-start text-[var(--text)]">
                <thead class="text-xs uppercase bg-[var(--bg-muted)] border-b border-[var(--border)] text-[var(--text-muted)]">
                  <tr>
                    <th class="px-4 py-3 font-semibold text-start">اليوم</th>
                    <th class="px-4 py-3 font-semibold text-start">الفترة</th>
                    <th class="px-4 py-3 font-semibold text-start">الوقت</th>
                    <th class="px-4 py-3 font-semibold text-start">الحالة</th>
                    <th class="px-4 py-3 font-semibold text-end">إجراءات</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-[var(--border)]">
                  @for (schedule of specSchedules(); track schedule.id) {
                    <tr class="bg-[var(--bg-surface)]">
                      <td class="px-4 py-3">{{ getDayName(schedule.dayOfWeek) }}</td>
                      <td class="px-4 py-3">
                        <span class="px-2 py-1 rounded-full text-xs font-medium"
                              [class]="schedule.period == 1 ? 'bg-amber-50 text-amber-600' : 'bg-indigo-50 text-indigo-600'">
                          {{ schedule.period == 1 ? 'صباحية' : 'مسائية' }}
                        </span>
                      </td>
                      <td class="px-4 py-3">{{ schedule.startTime }} - {{ schedule.endTime }}</td>
                      <td class="px-4 py-3">
                        <button (click)="toggleScheduleStatus(schedule.id!)" class="px-2 py-1 rounded-full text-xs font-medium transition-colors border"
                                [class]="schedule.isActive ? 'bg-emerald-50 text-emerald-600 border-emerald-200 hover:bg-emerald-100' : 'bg-red-50 text-red-600 border-red-200 hover:bg-red-100'">
                          {{ schedule.isActive ? 'نشط' : 'غير نشط' }}
                        </button>
                      </td>
                      <td class="px-4 py-3 text-end">
                        <sh-button variant="ghost" size="icon" (clicked)="removeSchedule(schedule.id!)">
                          <i class="fa-solid fa-trash text-[var(--text-muted)] hover:text-[var(--color-danger)]"></i>
                        </sh-button>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
        </sh-card>
      </div>
    }
  `
})
export class SpecializationsListComponent implements OnInit {
  private specsService = inject(SpecializationsService);
  private attachmentsService = inject(AttachmentsService);
  private fb = inject(FormBuilder);

  specializations = signal<Specialization[]>([]);
  isLoading = signal(true);
  
  searchTerm = '';
  currentPage = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  hasNext = signal(false);
  hasPrev = signal(false);

  // Main Modal State
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  editingId = signal<string | null>(null);
  apiError = signal<string | null>(null);
  selectedIconFile = signal<File | null>(null);
  iconPreviewUrl = signal<string | null>(null);
  currentIconAttachmentId = signal<string | null>(null);

  specForm = this.fb.group({
    name: ['', Validators.required],
    description: ['']
  });

  // Schedules Modal State
  showSchedulesModal = signal(false);
  selectedSpecForSchedules = signal<Specialization | null>(null);
  specSchedules = signal<SpecializationSchedule[]>([]);
  isLoadingSchedules = signal(false);
  isSavingSchedule = signal(false);

  scheduleForm = this.fb.group({
    dayOfWeek: [0, Validators.required],
    period: [1, Validators.required],
    startTime: ['', Validators.required],
    endTime: ['', Validators.required]
  });

  private searchTimeout: ReturnType<typeof setTimeout> | undefined;

  ngOnInit() {
    this.loadSpecializations();
  }

  getIconUrl(attachmentId: string): string {
    return this.attachmentsService.getDownloadUrl(attachmentId);
  }

  getDayName(day: number): string {
    const days = ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'];
    return days[day] || '';
  }

  loadSpecializations() {
    this.isLoading.set(true);
    this.specsService.getSpecializations({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm
    }).subscribe({
      next: (res) => {
        const isSuccess = res && (res.succeeded === true || (res as any).Succeeded === true || (res as any).isSuccess === true || res.succeeded === undefined);
        const data = res?.data || (res as any)?.Data || (res as any)?.value || (res as any)?.Value || res || null;
        
        if (isSuccess && data) {
          let items: Specialization[] = [];
          if (Array.isArray(data)) items = data;
          else if (Array.isArray(data.items)) items = data.items;
          else if (Array.isArray(data.Items)) items = data.Items;
          
          this.specializations.set(items);
          
          const pagination = data.pagination || data.Pagination;
          if (pagination) {
            this.totalPages.set(pagination.totalPages || pagination.TotalPages || 0);
            this.hasNext.set(pagination.hasNextPage || pagination.HasNextPage || false);
            this.hasPrev.set(pagination.hasPreviousPage || pagination.HasPreviousPage || false);
          }
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.specializations.set([]);
      }
    });
  }

  onSearch() {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage.set(1);
      this.loadSpecializations();
    }, 500);
  }

  changePage(delta: number) {
    this.currentPage.update(p => p + delta);
    this.loadSpecializations();
  }

  toggleStatus(id: string) {
    this.specsService.toggleStatus(id).subscribe({
      next: () => this.loadSpecializations(),
      error: () => Swal.fire('خطأ!', 'حدث خطأ أثناء تغيير حالة التخصص', 'error')
    });
  }

  // --- Main Modal Actions ---

  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      this.selectedIconFile.set(file);
      const reader = new FileReader();
      reader.onload = () => this.iconPreviewUrl.set(reader.result as string);
      reader.readAsDataURL(file);
    }
  }

  clearFile() {
    this.selectedIconFile.set(null);
    this.iconPreviewUrl.set(null);
    this.currentIconAttachmentId.set(null);
  }

  openModal(spec?: Specialization) {
    this.apiError.set(null);
    this.clearFile();
    
    if (spec) {
      this.isEditing.set(true);
      this.editingId.set(spec.id);
      this.specForm.patchValue({
        name: spec.name,
        description: spec.description || ''
      });
      if (spec.iconAttachmentId) {
        this.currentIconAttachmentId.set(spec.iconAttachmentId);
        this.iconPreviewUrl.set(this.getIconUrl(spec.iconAttachmentId));
      }
    } else {
      this.isEditing.set(false);
      this.editingId.set(null);
      this.specForm.reset();
    }
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.specForm.reset();
    this.isEditing.set(false);
    this.editingId.set(null);
    this.clearFile();
  }

  async onSubmit() {
    if (this.specForm.invalid) return;

    this.isSaving.set(true);
    this.apiError.set(null);
    const formValue = this.specForm.value;

    try {
      let finalAttachmentId = this.currentIconAttachmentId();

      if (this.selectedIconFile()) {
        const uploadRes = await firstValueFrom(
          this.attachmentsService.uploadAttachment(this.selectedIconFile()!, 'Specialization')
        );
        
        const isSuccess = uploadRes && (uploadRes.succeeded === true || (uploadRes as any).Succeeded === true || (uploadRes as any).isSuccess === true || uploadRes.succeeded === undefined);
        const data = uploadRes?.data || (uploadRes as any)?.Data || uploadRes || null;
        
        if (isSuccess && data?.attachmentId) {
          finalAttachmentId = data.attachmentId;
        } else if (isSuccess && data?.AttachmentId) {
          finalAttachmentId = data.AttachmentId;
        } else {
          throw new Error('فشل رفع الصورة');
        }
      }

      if (this.isEditing() && this.editingId()) {
        await firstValueFrom(
          this.specsService.updateSpecialization(this.editingId()!, formValue.name!, formValue.description || '', finalAttachmentId || undefined)
        );
      } else {
        await firstValueFrom(
          this.specsService.createSpecialization(formValue.name!, formValue.description || '', finalAttachmentId || undefined)
        );
      }

      this.closeModal();
      this.loadSpecializations();
    } catch (err: any) {
      this.apiError.set(err?.message || err?.error?.message || err?.error?.detail || err?.error?.title || 'حدث خطأ غير متوقع.');
    } finally {
      this.isSaving.set(false);
    }
  }

  deleteSpec(id: string) {
    Swal.fire({
      title: 'هل أنت متأكد؟',
      text: 'هل أنت متأكد من رغبتك في حذف هذا التخصص؟ لا يمكن التراجع عن هذا الإجراء.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#6b7280',
      confirmButtonText: 'نعم، احذفه!',
      cancelButtonText: 'إلغاء'
    }).then((result) => {
      if (result.isConfirmed) {
        this.specsService.deleteSpecialization(id).subscribe({
          next: (res) => {
            const isSuccess = res && (res.succeeded === true || (res as any).Succeeded === true || (res as any).isSuccess === true || res.succeeded === undefined);
            if (isSuccess) {
              this.loadSpecializations();
              Swal.fire('تم الحذف!', 'تم حذف التخصص بنجاح.', 'success');
            } else {
              Swal.fire('خطأ!', 'حدث خطأ أثناء الحذف.', 'error');
            }
          },
          error: () => Swal.fire('خطأ!', 'تعذر الحذف، قد يكون التخصص مستخدماً.', 'error')
        });
      }
    });
  }

  // --- Schedules Actions ---

  openSchedulesModal(spec: Specialization) {
    this.selectedSpecForSchedules.set(spec);
    this.showSchedulesModal.set(true);
    this.loadSchedules(spec.id);
  }

  closeSchedulesModal() {
    this.showSchedulesModal.set(false);
    this.selectedSpecForSchedules.set(null);
    this.specSchedules.set([]);
    this.scheduleForm.reset({ dayOfWeek: 0, period: 1 });
  }

  loadSchedules(specId: string) {
    this.isLoadingSchedules.set(true);
    this.specsService.getSpecializationById(specId).subscribe({
      next: (res) => {
        const data = res?.data || (res as any)?.Data || res || null;
        if (data) {
          const schedules = data.schedules || data.Schedules || [];
          this.specSchedules.set(schedules);
        }
        this.isLoadingSchedules.set(false);
      },
      error: () => {
        this.isLoadingSchedules.set(false);
        Swal.fire('خطأ!', 'تعذر تحميل المواعيد.', 'error');
      }
    });
  }

  onAddSchedule() {
    if (this.scheduleForm.invalid) return;
    
    const specId = this.selectedSpecForSchedules()?.id;
    if (!specId) return;

    this.isSavingSchedule.set(true);
    const formValue = this.scheduleForm.value;
    
    // Ensure format is HH:mm:ss for backend (if it requires seconds, we append :00)
    const start = formValue.startTime?.includes(':') && formValue.startTime.length === 5 ? `${formValue.startTime}:00` : formValue.startTime;
    const end = formValue.endTime?.includes(':') && formValue.endTime.length === 5 ? `${formValue.endTime}:00` : formValue.endTime;

    const payload: Partial<SpecializationSchedule> = {
      dayOfWeek: Number(formValue.dayOfWeek),
      period: Number(formValue.period),
      startTime: start!,
      endTime: end!
    };

    this.specsService.addSchedule(specId, payload).subscribe({
      next: () => {
        this.isSavingSchedule.set(false);
        this.scheduleForm.reset({ dayOfWeek: 0, period: 1 });
        this.loadSchedules(specId);
      },
      error: () => {
        this.isSavingSchedule.set(false);
        Swal.fire('خطأ!', 'حدث خطأ أثناء إضافة الموعد.', 'error');
      }
    });
  }

  removeSchedule(scheduleId: string) {
    const specId = this.selectedSpecForSchedules()?.id;
    if (!specId) return;

    Swal.fire({
      title: 'حذف الموعد',
      text: 'هل تريد فعلاً حذف هذا الموعد؟',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#6b7280',
      confirmButtonText: 'نعم، احذفه!',
      cancelButtonText: 'إلغاء'
    }).then((result) => {
      if (result.isConfirmed) {
        this.specsService.removeSchedule(specId, scheduleId).subscribe({
          next: () => this.loadSchedules(specId),
          error: () => Swal.fire('خطأ!', 'تعذر حذف الموعد.', 'error')
        });
      }
    });
  }

  toggleScheduleStatus(scheduleId: string) {
    const specId = this.selectedSpecForSchedules()?.id;
    if (!specId) return;

    this.specsService.toggleScheduleStatus(specId, scheduleId).subscribe({
      next: () => this.loadSchedules(specId),
      error: () => Swal.fire('خطأ!', 'تعذر تغيير حالة الموعد.', 'error')
    });
  }
}
