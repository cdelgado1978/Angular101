import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-quiz',
  templateUrl: './quiz.component.html',
  styleUrls: ['./quiz.component.css']
})
/** quiz component*/
export class QuizComponent {
  quiz: Quiz;
  /** quiz ctor */
  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string) {

    this.quiz = <Quiz>{};

    var id = +this.activatedRoute.snapshot.params["id"];
    console.log(id);

    if (id) {
      var url = this.baseUrl + "api/quiz/" + id;

      this.http.get<Quiz>(url).subscribe(result => {
          this.quiz = result;
      }, error => console.error(error))
    } else {
      console.log("Invalid id: routing back to home...");
      this.router.navigate(["home"]);
    }
  }
}