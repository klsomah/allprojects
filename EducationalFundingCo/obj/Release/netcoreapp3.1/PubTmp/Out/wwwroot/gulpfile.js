var gulp = require('gulp');

// Requires the gulp-sass plugin
var scss = require('gulp-sass');
var concat = require('gulp-concat');
livereload = require('gulp-livereload');


gulp.task('scss', function() {
    return gulp.src('app/scss/app.scss') // Gets all files ending with .scss in app/scss and children dirs
        .pipe(scss())
        .pipe(gulp.dest('app/css'))
});

gulp.task('watch', function() {
    gulp.watch('app/scss/**/*.scss', gulp.series('scss'));
    // Other watchers
});