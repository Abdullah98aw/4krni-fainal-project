import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Agency {
  id: number;
  name: string;
}

export interface Department {
  id: number;
  name: string;
  agencyId: number;
}

export interface Section {
  id: number;
  name: string;
  departmentId: number;
}

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAgencies(): Observable<Agency[]> {
    return this.http.get<Agency[]>(`${this.apiUrl}/organizations/agencies`);
  }

  getDepartmentsByAgency(agencyId: number): Observable<Department[]> {
    return this.http.get<Department[]>(
      `${this.apiUrl}/organizations/agencies/${agencyId}/departments`
    );
  }

  getSectionsByDepartment(departmentId: number): Observable<Section[]> {
    return this.http.get<Section[]>(
      `${this.apiUrl}/organizations/departments/${departmentId}/sections`
    );
  }
}
