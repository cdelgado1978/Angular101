import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-quiz-edit',
    templateUrl: './quiz-edit.component.html',
    styleUrls: ['./quiz-edit.component.css']
})
/** quiz-edit component*/
export class QuizEditComponent {
  title: string;
  quiz: Quiz;

  editMode: boolean;

  /** quiz-edit ctor */
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {

    this.quiz = <Quiz>{};

    var id = +this.activatedRoute.snapshot.params["id"];

    if (id) {
      this.editMode = true;

      var url = this.baseUrl + "api/quiz/" + id;
      this.http.get<Quiz>(url).subscribe(res => {
        this.quiz = res;
        this.title = "Edit - " + this.quiz.Title;
      }, error => console.error(error));
    }
    else {
      this.editMode = false;
      this.title = "Create a new Quiz";
    }

  }

  onSubmit(quiz: Quiz) {
    var url = this.baseUrl + "api/quiz";

    if (this.editMode) {
      this.http
        .put<Quiz>(url, quiz)
        .subscribe(res => {
          var v = res;
          console.log("Quiz " + v.Id + " has been updated.");
          this.router.navigate(["/home"]);
        }, error => console.log(error));
    } else {
      this.http
        .post<Quiz>(url, quiz)
        .subscribe(res => {
          var q = res;
          console.log("Quiz " + q.Id + " has beed created.");
          this.router.navigate(["/home"]);

        }, error => console.log(error));
    }
  }

  onBack() {
    this.router.navigate(["/home"]);
  }

}
