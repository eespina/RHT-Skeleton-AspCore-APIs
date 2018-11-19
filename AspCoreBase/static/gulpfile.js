var autoprefixer = require('gulp-autoprefixer');
var cssmin = require('gulp-cssmin');
var concat = require('gulp-concat');
var del = require('del');
var errorHandler = require('gulp-error-handle');
var gulp = require('gulp');
var gutil = require('gulp-util');
var jshint = require('gulp-jshint');
var nib = require('nib');
var rename = require('gulp-rename');
var runSequence = require('run-sequence');
var sass = require('gulp-sass');
var stripCss = require('gulp-strip-css-comments');
var stripDebug = require('gulp-strip-debug');
var stylus = require('gulp-stylus');
var uglify = require('gulp-uglify');
var vinylPaths = require('vinyl-paths');

var paths = {
	js: 'src/js/*.js',
	jsConcatSrc: ['!src/js/scripts.concat.js', 'src/js/*.js'],
	jsDist: '../wwwroot/dist/js',
	jsSrc: 'src/js',
	jsConcatFilename: 'scripts.concat.js',
	jsMinFilename: 'scripts.min.js',
	css: 'css',
	cssDist: '../wwwroot/dist/css',
	cssSrc: 'src/css',
	cssWatch: 'src/styl/**/*.styl',
	cssWatch2: 'src/styl/**/**/*.styl',
	cssMinFilename: 'main.min.css',
	cssConcatFilename: 'main.concat.css',
	cssStylFilename: '_main-styl.scss',
	mainStyl: 'src/styl/main.styl',
     jqueryFilepath: 'node_modules/jquery/dist/jquery.min.js',
     jqueryMinFilename: 'jquery.min.js'
};

gulp.task('default', function (callback) {
	runSequence('clean', 'styles', 'js', 'watch:styles', 'watch:js', callback);
});

gulp.task('build', function (callback) {
	runSequence('clean', 'styles', 'js', callback);
});

gulp.task('clean', function (callback) {
	runSequence('clean:dist', 'clean:src', 'clean:js', callback);
});

gulp.task('styles', function (callback) {
	runSequence('stylus', 'sass', 'prefix', callback);
});

gulp.task('js', function (callback) {
	runSequence('lint', 'scripts:main', 'scripts:jquery', callback);
});

gulp.task('watch:styles', function () {
	gulp.watch([paths.cssWatch, paths.cssWatch2], ['styles']);
});
gulp.task('watch:js', function () {
	gulp.watch(paths.js, ['lint', 'scripts:main']);
});

gulp.task('stylus', function () {
	return gulp.src(paths.mainStyl)
		.pipe(stylus({
			import: ['nib'],
			use: [nib()],
			'include css': true
		}))
		.pipe(errorHandler())
		.pipe(rename(paths.cssStylFilename))
		.pipe(gulp.dest(paths.cssSrc));
});

gulp.task('sass', function () {
	return gulp.src('src/sass/main.scss')
		.pipe(sass())
		.pipe(stripCss())
		.pipe(rename(paths.cssConcatFilename))
		.pipe(gulp.dest(paths.cssSrc))
		.pipe(cssmin())
		.pipe(rename(paths.cssMinFilename))
		.pipe(gulp.dest(paths.cssDist));
});

gulp.task('prefix', function () {
	return gulp.src('src/css/main.concat.css')
		.pipe(autoprefixer({
			browsers: ['last 2 versions'],
			cascade: false
		}))
		.pipe(rename(paths.cssConcatFilename))
		.pipe(gulp.dest(paths.cssSrc))
		.pipe(cssmin())
		.pipe(rename(paths.cssMinFilename))
		.pipe(gulp.dest(paths.cssDist));
});

gulp.task('serve', ['js'], function () {
	gulp.watch("js/*.js", ['js-watch']);
});

gulp.task('lint', function () {
	return gulp.src(['gulpfile.js', paths.js])
		.pipe(jshint('.jshintrc'))
		.pipe(jshint.reporter('default'));
});

gulp.task('scripts:main', function () {
	return gulp.src(paths.jsConcatSrc)
		.pipe(concat(paths.jsConcatFilename))
		.pipe(gulp.dest(paths.jsSrc))
		.pipe(stripDebug())
		.pipe(uglify())
		.pipe(rename(paths.jsMinFilename))
		.pipe(gulp.dest(paths.jsDist));
});

gulp.task('scripts:jquery', function() {
  return gulp.src(paths.jqueryFilepath)
    // Strip all debugger code out.
    .pipe(stripDebug())
    // Minify the JavaScript.
    .pipe(uglify())
    .pipe(rename(paths.jqueryMinFilename))
    .pipe(gulp.dest(paths.jsDist +'/vendor/'));
});

gulp.task('clean:dist', function () {
	return gulp.src('dist/*')
		.pipe(vinylPaths(del))
		.pipe(stripDebug())
		.pipe(gulp.dest('dist'));
});
gulp.task('clean:src', function () {
	return gulp.src(paths.cssSrc + "/" + paths.cssStylFilename)
		.pipe(vinylPaths(del))
		.pipe(stripDebug())
		.pipe(gulp.dest('src'));
});
gulp.task('clean:js', function () {
	return gulp.src(paths.jsSrc + "/" + paths.jsConcatFilename)
		.pipe(vinylPaths(del))
		.pipe(stripDebug())
		.pipe(gulp.dest('src'));
});