import { Component, OnInit } from '@angular/core';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  activeTab: 'users' | 'locations' = 'users';
  users: any[] = [];
  locations: any[] = [];
  currentPage = 1;
  itemsPerPage = 10;
  totalItems = 0;
  errorMessage = '';
  Math = Math;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.adminService.getUsers(this.currentPage, this.itemsPerPage).subscribe(
      (data) => {
        this.users = data.users;
        this.totalItems = data.totalCount;
      },
      (error) => {
        console.error('Error loading users:', error);
        this.errorMessage = 'Failed to load users. Please try again.';
      }
    );
  }

  loadLocations(): void {
    this.adminService.getLocations(this.currentPage, this.itemsPerPage).subscribe(
      (data) => {
        this.locations = data.locations;
        this.totalItems = data.totalCount;
      },
      (error) => {
        console.error('Error loading locations:', error);
        this.errorMessage = 'Failed to load locations. Please try again.';
      }
    );
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    if (this.activeTab === 'users') {
      this.loadUsers();
    } else {
      this.loadLocations();
    }
  }

  switchTab(tab: 'users' | 'locations'): void {
    this.activeTab = tab;
    this.currentPage = 1;
    if (tab === 'users') {
      this.loadUsers();
    } else {
      this.loadLocations();
    }
  }

  updateUser(userId: string, userData: any): void {
    this.adminService.updateUser(userId, userData).subscribe(
      () => {
        this.loadUsers();
      },
      (error) => {
        console.error('Error updating user:', error);
        this.errorMessage = 'Failed to update user. Please try again.';
      }
    );
  }

  deleteUser(userId: string): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.adminService.deleteUser(userId).subscribe(
        () => {
          this.loadUsers();
        },
        (error) => {
          console.error('Error deleting user:', error);
          this.errorMessage = 'Failed to delete user. Please try again.';
        }
      );
    }
  }

  updateLocation(locationId: string, locationData: any): void {
    this.adminService.updateLocation(locationId, locationData).subscribe(
      () => {
        this.loadLocations();
      },
      (error) => {
        console.error('Error updating location:', error);
        this.errorMessage = 'Failed to update location. Please try again.';
      }
    );
  }

  deleteLocation(locationId: string): void {
    if (confirm('Are you sure you want to delete this location?')) {
      this.adminService.deleteLocation(locationId).subscribe(
        () => {
          this.loadLocations();
        },
        (error) => {
          console.error('Error deleting location:', error);
          this.errorMessage = 'Failed to delete location. Please try again.';
        }
      );
    }
  }
}
