import { Component, OnInit, ViewChild } from '@angular/core';
import {
  GoogleMap,
  GoogleMapsModule,
  MapInfoWindow,
} from '@angular/google-maps';
import { LocationService } from '../../services/location.service';
import { MapService } from 'src/app/services/map.service';
import { CommonModule } from '@angular/common';
import { SearchComponent } from '../search/search.component';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LocationDetailComponent } from '../location-detail/location-detail.component';

interface Marker {
  pin?: google.maps.marker.AdvancedMarkerElement;
  info: any;
}

@Component({
  selector: 'app-map',
  standalone: true,
  imports: [
    CommonModule,
    GoogleMapsModule,
    SearchComponent,
    RouterModule,
    FormsModule,
  ],
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss'],
})
export class MapComponent implements OnInit {
  @ViewChild(GoogleMap) map!: GoogleMap;
  @ViewChild(MapInfoWindow) infoWindow!: MapInfoWindow;

  private geocoder: google.maps.Geocoder;

  center: google.maps.LatLngLiteral = { lat: 16.0544, lng: 108.2022 }; // Da Nang, Vietnam
  zoom = 7;
  markerOptions: google.maps.MarkerOptions = { draggable: false };
  markers: Marker[] = [];
  addMarkerForm: boolean = false;
  selectedMarker: Marker = { info: {} };

  constructor(
    private locationService: LocationService,
    private modalService: NgbModal,
    private mapService: MapService,
    private authService: AuthService
  ) {
    this.geocoder = new google.maps.Geocoder();
  }

  ngOnInit(): void {
    this.zoomToCurrentLocation();
    this.loadLocations();
  }

  loadLocations(): void {
    this.locationService.getLocations().subscribe(
      (locations) => {
        this.updateMarkers(locations);
      },
      (error) => {
        console.error('Error loading locations:', error);
      }
    );
  }

  searchResultSelected(result: any): void {
    this.center = {lat: result.latitude, lng: result.longitude};
    this.zoom = 18;
    this.openInfoWindow(this.markers.find(x => x.info.id === result.id)!);
  }

  onFilterResults(results: any[]): void {
    this.updateMarkers(results);
  }

  updateMarkers(locations: any[]): void {
    locations.forEach((location) => {
      const marker: Marker = {
        pin: new google.maps.marker.AdvancedMarkerElement({
          position: {
            lat: location.latitude,
            lng: location.longitude,
          },
          title: location.name,
          content: new google.maps.marker.PinElement({
            background: '#0A5EF0',
          }).element,
        }),
        info: location,
      };

      marker.pin?.addListener('click', () => {
        this.openInfoWindow(marker);
      });

      this.markers.push(marker);
    });
  }

  openInfoWindow(marker: Marker): void {
    this.infoWindow.close();
    const content = document.createElement('div');
    this.authService.isAuthenticated().subscribe((isAuthenticated) => {
      content.innerHTML = `
          <h6>${marker.info?.name || marker.pin?.title|| `New Location`}</h6>
          ${
            marker.info?.id
              ? `
              <p>Category: ${marker.info?.category}</p>
          <p>Date Added: ${new Date(
            marker.info?.createdAt
          ).toDateString()}</p>
          <button class="btn btn-primary view-details">View Details</button>`
          // <a href="/location/${
          //         marker.info?.id
          //       }" class="btn btn-primary">View Details</a>
          //       `
              : isAuthenticated
              ? `<p>This location has not been marked yet. Do you want to share this location? </p>`
              : `<p>This location has not been marked yet. Please login to share this location.</p>`
          }
    `;
    this.addMarkerForm = !marker.info?.id && isAuthenticated;
    });
    const infoWindow = new google.maps.InfoWindow({
      content: content,
      pixelOffset: new google.maps.Size(0, -40),
      
    });
    this.infoWindow.infoWindow = infoWindow;
    this.infoWindow.infoWindow.addListener('domready', () => {
      document.querySelector('.view-details')?.addEventListener('click', () => {
        this.openLocationDetailsPopup(marker.info);
      });
    });
    this.infoWindow.openAdvancedMarkerElement(marker.pin!);
  }

  addMarker(event: google.maps.MapMouseEvent): void {
    if (event.latLng) {
      this.geocoder.geocode({ location: event.latLng }, (results, status) => {
        if (status === 'OK' && results?.[0]) {
          this.selectedMarker.pin =
            new google.maps.marker.AdvancedMarkerElement({
              position: event.latLng,
              content: new google.maps.marker.PinElement({
                background: '#0A5EF0',
              }).element,
              title: results[0].formatted_address,
            });
            this.selectedMarker.info.name = results[0].formatted_address;
        } else {
          this.selectedMarker.pin =
            new google.maps.marker.AdvancedMarkerElement({
              position: event.latLng,
              content: new google.maps.marker.PinElement({
                background: '#0A5EF0',
              }).element,
            });
        }
        this.openInfoWindow(this.selectedMarker);
      });
    }
  }

  savePin(): void {
    if (this.addMarkerForm) {
      console.log(this.selectedMarker.info);
      this.mapService
        .addPin(
          this.selectedMarker.pin!.position!.lat as number,
          this.selectedMarker.pin!.position!.lng as number,
          this.selectedMarker.pin!.title,
          this.selectedMarker.info?.description,
          this.selectedMarker.info?.category
        )
        .subscribe(
          (response) => {
            this.selectedMarker.info = response;
            this.markers.push(this.selectedMarker);
            this.selectedMarker = { info: {} };
            this.infoWindow.close();
            this.addMarkerForm = false;
          },
          (error) => {
            console.error('Error saving pin:', error);
          }
        );
    }
  }

  zoomToCurrentLocation() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const pos = {
            lat: position.coords.latitude,
            lng: position.coords.longitude,
          };
          this.center = pos;
          this.zoom = 16; // You can adjust this zoom level as needed
        },
        () => {
          this.handleLocationError(true);
        }
      );
    } else {
      this.handleLocationError(false);
    }
  }

  handleLocationError(browserHasGeolocation: boolean) {
    console.log(
      browserHasGeolocation
        ? 'Error: The Geolocation service failed.'
        : "Error: Your browser doesn't support geolocation."
    );
  }

  openLocationDetailsPopup(location: any) {
    const modalRef = this.modalService.open(LocationDetailComponent, {
      size: 'lg',
      windowClass: 'custom-modal-class'
    });
    modalRef.componentInstance.location = location;
  }
}
