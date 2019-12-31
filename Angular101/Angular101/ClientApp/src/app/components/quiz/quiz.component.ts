import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-quiz',
    templateUrl: './quiz.component.html',
    styleUrls: ['./quiz.component.css']
})
/** quiz component*/
export class QuizComponent {
  @Input() quiz: Quiz;
    /** quiz ctor */
    constructor() {

    }
}
