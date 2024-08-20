import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.scss']
})
export class FeedbackComponent implements OnInit {
  @Input() feedback: any;

  constructor() { }

  ngOnInit(): void { }

  getStarArray(rating: number): number[] {
    return Array(Math.round(rating)).fill(0);
  }
}
